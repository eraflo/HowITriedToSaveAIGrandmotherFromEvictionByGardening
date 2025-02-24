using Assets.Scripts.GeminiAI.Request;
using Assets.Scripts.GeminiAI.Request.RequestParts;
using Assets.Scripts.GeminiAI.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GeminiAI
{
    [Serializable]
    public class GeminiOptions
    {
        public string ApiKey;
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

        private Action<string> _onResponseReceived;
        private Action<string> _onOpenConnexion;
        private Action<string> _onCloseConnexion;
        private Action<string> _onMessageReceived;

        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.None
        };

        public ConnexionType ConnexionType { get; set; } = ConnexionType.Http;

        public AIGenerationConfig GenerationConfig { get; set; } = new();
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

            // Http client for simple API requests like generating text content
            if (_httpClient == null)
            {
                _httpClient = new HttpClient(new GeminiDelegatingHandler(_options));
            }

            // WebSocket client for real-time interactions and the multi-modal API
            if (_webSocket == null)
            {
                _webSocket = new ClientWebSocket();

                _onResponseReceived += OnResponseReceived;
                _onOpenConnexion += OnOpenConnexion;
                _onCloseConnexion += OnCloseConnexion;
                _onMessageReceived += OnMessageReceived;
            }
        }

        #region WebSocket Action

        public async Task Connect(string url, string key, string model)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                return;
            }


            Debug.Log("Connecting to Gemini WebSocket");

            await _webSocket.ConnectAsync(new Uri(url + $"?key={key}"), CancellationToken.None);

            Task.Run(() => Receive());
        }

        public async Task Send(string json, WebSocketMessageType messageType)
        {
            var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(json));
            await _webSocket.SendAsync(buffer, messageType, true, CancellationToken.None);
        }

        public async Task Receive()
        {
            while (true) {
                try
                {
                    var responseBuffer = new ArraySegment<byte>(new byte[1024]);
                    var response = "";

                    do
                    {
                        var result = await _webSocket.ReceiveAsync(responseBuffer, CancellationToken.None);
                        response += System.Text.Encoding.UTF8.GetString(responseBuffer.Array, 0, result.Count);
                    } while (!_webSocket.CloseStatus.HasValue);

                    _onResponseReceived?.Invoke(response);

                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error receiving message: {ex.Message}");
                    Debug.LogError(ex.StackTrace);
                }
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
        public void OnResponseReceived(string onResponseReceived)
        {
            switch (_webSocket.State)
            {
                case WebSocketState.Open:
                    var response = onResponseReceived;
                    if (response.Contains("message"))
                    {
                        _onMessageReceived?.Invoke(onResponseReceived);
                    }
                    else
                    {
                        _onOpenConnexion?.Invoke(onResponseReceived);
                    }
                    break;
                case WebSocketState.Closed:
                    _onCloseConnexion?.Invoke(onResponseReceived);
                    break;
                default:
                    Debug.LogError("WebSocket connection is not open.");
                    break;
            }
        }

        public void OnOpenConnexion(string response)
        {
            var setup = new SetupAI
            {
                Setup = new ModelAI
                {
                    Model = response
                }
            };

            var setupJson = JsonConvert.SerializeObject(setup, _serializerSettings);

            Task.Run(() => Send(setupJson, WebSocketMessageType.Text));
        }

        public void OnCloseConnexion(string response)
        {
            Debug.Log("WebSocket connection closed.");
        }

        public void OnMessageReceived(string response)
        {
            Debug.Log(response);
        }

        #endregion

        public Task GenerateMultiModalRequestAsync(string input)
        {
            try
            {
                var requestBody = APIRequestFactory.CreateRequest(
                    prompt: input,
                    parts: new RequestPart[]
                    {
                        new TextPart
                        {
                            Text = input
                        }
                    },
                    generationConfig: GenerationConfig,
                    safetySettings: SafetySettings.ToArray()
                );

                var serializedRequestBody = JsonConvert.SerializeObject(requestBody, _serializerSettings);

                Task.Run(() => Send(serializedRequestBody, WebSocketMessageType.Text));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error generating content: {ex.Message}");
                Debug.LogError(ex.StackTrace);
            }

            return Task.CompletedTask;
        }


        public async Task<string> GenerateTextContentAsync(string input, CancellationToken cancellationToken)
        {
            try
            {
                var requestBody = APIRequestFactory.CreateRequest(
                    prompt: input, 
                    parts: new RequestPart[]
                    {
                        new TextPart
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
                var geminiResponse = JsonConvert.DeserializeObject<AIResponse>(responseBody);

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
