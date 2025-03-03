using Assets.Scripts.GeminiAI.Enums;
using Assets.Scripts.GeminiAI.FunctionsCall;
using Assets.Scripts.GeminiAI.Request;
using Assets.Scripts.GeminiAI.Request.Bidi;
using Assets.Scripts.GeminiAI.Parts;
using Assets.Scripts.GeminiAI.Response;
using Assets.Scripts.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

namespace Assets.Scripts.GeminiAI
{
    [Serializable]
    public class GeminiOptions
    {
        public string Model;
        public string Interaction;
        public string ApiKey;

        [Tooltip("Don't do anything for http connexion")]
        public bool HasSession;

        [HideInInspector]
        public string Url;
    }

    public enum ConnexionType
    {
        Http,
        WebSocket
    }

    [CreateAssetMenu(fileName = "GeminiClient", menuName = "Gemini/GeminiClient")]
    public class GeminiClient : ScriptableObject
    {
        [SerializeField] private GeminiOptions _options = new();

        private HttpClient _httpClient;
        private ClientWebSocket _webSocket;

        private Action<string> _onConnected;
        private Action<string> _onCloseConnexion;
        private Action<string> _onResponseReceived;
        private Action<string> _onContentReceived;

        private Tool _tool = new();

        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _hasSendRequest = false;
        private bool _isConnected = false;

        public ConnexionType ConnexionType { get; set; } = ConnexionType.Http;

        public GenerationConfig GenerationConfig { get; set; } = new();
        public List<SafetySettings> SafetySettings { get; set; } = new();


        public GeminiOptions Options { get => _options; }


        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_options.ApiKey))
            {
                Debug.LogError("Gemini API Key is required.");
            }

            if (string.IsNullOrEmpty(_options.Url))
            {
                Debug.LogError("Gemini URL is required.");
            }
        }

        private void OnEnable()
        {

            // Http client for simple API requests like generating text content
            if (_httpClient == null)
            {
                _httpClient = new HttpClient(new GeminiDelegatingHandler(_options));
            }

            // WebSocket client for real-time interactions and the multi-modal API
            if (_webSocket == null)
            {
                _webSocket = new ClientWebSocket();

                _onConnected += OnConnected;
                _onResponseReceived += OnResponseReceived;
                _onContentReceived += OnContentReceived;
            }

            GenerateURL();

            Debug.Log($"Gemini URL: {_options.Url}");
        }

        private async void OnDisable()
        {
            await Disconnect();

            _httpClient?.Dispose();
            _webSocket?.Dispose();

            _onConnected -= OnConnected;
            _onResponseReceived -= OnResponseReceived;
            _onContentReceived -= OnContentReceived;
        }

        public void ChangeConnexionType(ConnexionType connexionType, string interaction)
        {
            ConnexionType = connexionType;
            _options.Interaction = interaction;

            GenerateURL();
        }

        public T GetResponseObject<T>(string response)
        {
            return JsonConvert.DeserializeObject<T>(response);
        }

        private void GenerateURL()
        {
            switch (ConnexionType)
            {
                case ConnexionType.Http:
                    _options.Url = "https://generativelanguage.googleapis.com/v1beta/";
                    _options.Url += _options.Model + ":" + _options.Interaction;
                    break;
                case ConnexionType.WebSocket:
                    _options.Url = "wss://generativelanguage.googleapis.com/ws/google.ai.generativelanguage.v1alpha.GenerativeService.";
                    _options.Url += _options.Interaction;
                    break;
            }
        }

        #region WebSocket Action

        public async Task Connect(string model, bool hasSession, string context = "")
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                _isConnected = true;
                return;
            }

            _isConnected = false;

            Debug.Log("Connecting to Gemini WebSocket");

            var url = _options.Url;
            var key = _options.ApiKey;

            url += $"?key={key}";

            if(hasSession)
            {
                url += $"&sessionId={Guid.NewGuid()}";
            }

            await _webSocket.ConnectAsync(new Uri(url), new CancellationTokenSource().Token);

            Task.Run(() => Receive());

            var setup = new SendMessage
            {
                Setup = new BidiGenerateContentSetup
                {
                    Model = model,
                    GenerationConfig = GenerationConfig == null ? null : GenerationConfig,
                    SystemInstruction = new Content
                    {
                        Parts = new Part[]
                        {
                            new Part
                            {
                                Text = context
                            }
                        }
                    },
                    Tools = _tool == null ? null : new Tool[] { _tool }
                }
            };

            Task.Run(() => Send(setup, WebSocketMessageType.Text));
        }

        public async Task Connect(string model, string context = "")
        {
            await Connect(model, _options.HasSession, context);
        }

        public async Task Send(SendMessage message, WebSocketMessageType messageType)
        {
            // Make sure only one thread is sending messages at a time
            await _semaphore.WaitAsync();
            try
            {
                var json = JsonConvert.SerializeObject(message, _serializerSettings);

                //Debug.Log(json);

                _hasSendRequest = true;

                var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(json));
                await _webSocket.SendAsync(buffer, messageType, true, new CancellationTokenSource().Token);
            }
            finally
            {
                _semaphore.Release();
            }

        }

        private async Task Receive()
        {
            var continueReceiving = true;
            var lastResponse = "";
            var closeStatus = WebSocketCloseStatus.Empty;
            var closeStatusDescription = "";
            var messageType = WebSocketMessageType.Text;


            while (continueReceiving) {
                try
                {
                    if(!_hasSendRequest)
                    {
                        await Task.Delay(100);
                        continue;
                    }

                    var byteArray = new byte[GenerationConfig.MaxOutputTokens];
                    var responseBuffer = new ArraySegment<byte>(byteArray);
                    var response = "";
                    var continueGettingData = true;

                    do
                    {
                        var result = await _webSocket.ReceiveAsync(responseBuffer, new CancellationTokenSource().Token);
                        response += System.Text.Encoding.UTF8.GetString(responseBuffer.Array, 0, result.Count);

                        if (result.CloseStatus != null)
                            closeStatus = result.CloseStatus.Value;

                        //Debug.Log(response);

                        closeStatusDescription = result.CloseStatusDescription;
                        messageType = result.MessageType;

                        //Debug.Log($"Message type: {messageType}");
                        //Debug.Log($"Close status: {closeStatus}");
                        //Debug.Log($"Close status description: {closeStatusDescription}");

                        if (messageType.ToString() == "Closed" || messageType.ToString() == "Close")
                        {
                            continueReceiving = false;
                            _onResponseReceived?.Invoke(response);
                            break;
                        }


                        continueGettingData = !result.EndOfMessage;
                    } while (continueGettingData);

                    if (response != lastResponse && !string.IsNullOrEmpty(response))
                    {
                        _onResponseReceived?.Invoke(response);
                        lastResponse = response;
                    }

                    // Free the buffer
                    Array.Clear(responseBuffer.Array, 0, responseBuffer.Count);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error receiving message: {ex.Message}");
                    Debug.LogError(ex.StackTrace);
                    Debug.LogError($"Close status: {closeStatus}");
                    Debug.LogError($"Close status description: {closeStatusDescription}");
                    Debug.LogError($"Message type: {messageType}");
                    continueReceiving = false;
                }

                await Task.Delay(100);
            }
        }

        public async Task Disconnect()
        {
            if (_webSocket.State == WebSocketState.Closed)
            {
                return;
            }

            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
        }

        #endregion

        #region WebSocket Events

        private void OnResponseReceived(string onResponseReceived)
        {
            Debug.Log(_webSocket.State);
            switch (_webSocket.State)
            {
                case WebSocketState.Open:

                    // First response is the connection confirmation
                    if (!_isConnected)
                        _onConnected?.Invoke(onResponseReceived);
                    else
                        _onContentReceived?.Invoke(onResponseReceived);
                    break;
                case WebSocketState.CloseSent:
                case WebSocketState.Closed:
                case WebSocketState.CloseReceived:
                    _onCloseConnexion?.Invoke(onResponseReceived);
                    break;
                default:
                    Debug.LogError("WebSocket connection is not open.");
                    break;
            }
        }



        private void OnConnected(string response)
        {
            Debug.Log("WebSocket connection established.");
            Debug.Log(response);
            _onConnected -= OnConnected;
            _onCloseConnexion += OnCloseConnexion;

            _isConnected = true;
        }

        private void OnCloseConnexion(string response)
        {
            Debug.Log("WebSocket connection closed.");
            _onCloseConnexion -= OnCloseConnexion;
            _onConnected += OnConnected;

            _isConnected = false;
        }

        private void OnContentReceived(string response)
        {

        }



        public void ListenToContentReceived(Action<string> onContentReceived)
        {
            _onContentReceived += onContentReceived;
        }

        public void ListenToCloseConnexion(Action<string> onCloseConnexion)
        {
            _onCloseConnexion += onCloseConnexion;
        }

        public void ListenToConnected(Action<string> onResponseReceived)
        {
            _onConnected += onResponseReceived;
        }



        public void RemoveContentReceivedListener(Action<string> onContentReceived)
        {
            _onContentReceived -= onContentReceived;
        }

        public void RemoveCloseConnexionListener(Action<string> onCloseConnexion)
        {
            _onCloseConnexion -= onCloseConnexion;
        }

        public void RemoveConnectedListener(Action<string> onResponseReceived)
        {
            _onConnected -= onResponseReceived;
        }

        #endregion

        public Task GenerateMultiModalRequestAsync(string input)
        {
            try
            {
                var requestBody = new SendMessage
                {
                    ClientContent = new BidiGenerateContentClientContent
                    {
                        Turns = new Content[]
                        {
                            new Content
                            {
                                Role = Role.User,
                                Parts = new Part[]
                                {
                                    new Part
                                    {
                                        Text = input
                                    }
                                }
                            }

                        },
                        TurnComplete = true
                    }
                };

                Task.Run(() => Send(requestBody, WebSocketMessageType.Text));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error generating content: {ex.Message}");
                Debug.LogError(ex.StackTrace);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Declare a new function that can be used in the generation of the content
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="parameter"></param>
        public void DeclareNewFunction(string name, string description = null, Parameter parameter = null)
        {
            var newFunctionCall = new FunctionDeclaration()
            {
                Name = name,
                Description = description,
                Parameters = parameter
            };

            if (_tool.FunctionDeclarations == null)
            {
                _tool.FunctionDeclarations = new FunctionDeclaration[] { newFunctionCall };
                return;
            }

            _tool.FunctionDeclarations = ArrayUtils.Add(_tool.FunctionDeclarations, newFunctionCall);
        }

        public void DeclareNewFunction(FunctionDeclaration func)
        {
            if (_tool.FunctionDeclarations == null)
            {
                _tool.FunctionDeclarations = new FunctionDeclaration[] { func };
                return;
            }

            _tool.FunctionDeclarations = ArrayUtils.Add(_tool.FunctionDeclarations, func);
        }


        public async Task<string> GenerateTextContentAsync(string input, CancellationToken cancellationToken)
        {
            try
            {
                var requestBody = APIRequestFactory.CreateRequest(
                    prompt: input, 
                    parts: new Part[]
                    {
                        new Part
                        {
                            Text = input
                        }
                    },
                    generationConfig: GenerationConfig, 
                    safetySettings: SafetySettings.ToArray()
                );

                var serializedRequestBody = JsonConvert.SerializeObject(requestBody, _serializerSettings);
                var content = new StringContent(serializedRequestBody, System.Text.Encoding.UTF8, "application/json");


                var response = await _httpClient.PostAsync(_options.Url, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                // Read the response body
                var responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the response
                var geminiResponse = GetResponseObject<GenerateContentResponse>(responseBody);

                // Get the text from the response
                var geminiResponseText = geminiResponse?.Candidates[0].Content.Parts[0].Text;

                return geminiResponseText;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error generating content: {ex.Message}");
                Debug.LogError(ex.StackTrace);
                return null;
            }
        }
    }
}
