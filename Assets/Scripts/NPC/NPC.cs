using Assets.Scripts.Audio;
using Assets.Scripts.GeminiAI;
using Assets.Scripts.GeminiAI.Enums;
using Assets.Scripts.GeminiAI.FunctionsCall;
using Assets.Scripts.GeminiAI.Request;
using Assets.Scripts.GeminiAI.Request.Bidi;
using Assets.Scripts.GeminiAI.Parts;
using Assets.Scripts.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using System.Reflection;
using System.Globalization;
using Assets.Scripts.NPC.Behaviour;
using System.Threading;

namespace Assets.Scripts.NPC
{
    [RequireComponent(typeof(AudioRecorder), typeof(AudioSpeaker), typeof(BehaviourTree))]
    public class NPC : MonoBehaviour
    {
        [Header("Gemini Client")]
        [SerializeField] protected GeminiClient _geminiClient;
        [SerializeField] protected string model;
        [SerializeField] protected PrebuiltVoice _voice;
        [SerializeField, TextArea(1, 100000, order = 0)] private string _context;

        protected ToolArea _toolArea;

        protected AudioRecorder _audioRecorder;
        protected AudioSpeaker _audioSpeaker;

        protected byte[] audioAnswer;
        protected bool hasAudioAnswer = false;

        protected BehaviourTree _behaviourTree;

        private Dictionary<string, CancellationTokenSource> _taskRunning = new Dictionary<string, CancellationTokenSource>();

        private Action OnComplete;

        private async void Awake()
        {
            if (_geminiClient == null)
            {
                throw new Exception("Gemini Client is not set.");
            }

            OnComplete += onTurnComplete;

            _audioRecorder = GetComponent<AudioRecorder>();
            _audioRecorder.onGetAudioFlux += OnGetAudioFlux;

            _audioSpeaker = GetComponent<AudioSpeaker>();

            _behaviourTree = GetComponent<BehaviourTree>();

            _geminiClient.ChangeConnexionType(ConnexionType.WebSocket, "BidiGenerateContent");
            _geminiClient.GenerationConfig.ResponseMimeType = MimeType.TEXT;
            _geminiClient.GenerationConfig.ResponseModalities = new ResponseModality[1] { ResponseModality.AUDIO };
            _geminiClient.GenerationConfig.MaxOutputTokens = 2048;
            _geminiClient.GenerationConfig.CandidateCount = 1;
            _geminiClient.GenerationConfig.Temperature = 1f;
            _geminiClient.GenerationConfig.TopP = 1f;
            _geminiClient.GenerationConfig.TopK = 1;

            _geminiClient.GenerationConfig.SpeechConfig = new();
            _geminiClient.GenerationConfig.SpeechConfig.VoiceConfig = new();
            _geminiClient.GenerationConfig.SpeechConfig.VoiceConfig.PrebuiltVoiceConfig = new();
            _geminiClient.GenerationConfig.SpeechConfig.VoiceConfig.PrebuiltVoiceConfig.VoiceName = _voice;

            _geminiClient.SafetySettings = new List<SafetySettings>()
            {
                new SafetySettings()
                {
                    Category = HarmCategory.HARM_CATEGORY_HARASSMENT,
                    Threshold = HarmThreshold.BLOCK_ONLY_HIGH
                },
                new SafetySettings()
                {
                    Category = HarmCategory.HARM_CATEGORY_HATE_SPEECH,
                    Threshold = HarmThreshold.BLOCK_ONLY_HIGH
                },
                new SafetySettings()
                {
                    Category = HarmCategory.HARM_CATEGORY_SEXUALLY_EXPLICIT,
                    Threshold = HarmThreshold.BLOCK_ONLY_HIGH
                },
                new SafetySettings()
                {
                    Category = HarmCategory.HARM_CATEGORY_DANGEROUS_CONTENT,
                    Threshold = HarmThreshold.BLOCK_ONLY_HIGH
                },
                new SafetySettings()
                {
                    Category = HarmCategory.HARM_CATEGORY_CIVIC_INTEGRITY,
                    Threshold = HarmThreshold.BLOCK_ONLY_HIGH
                }
            };


            DeclareFunctions();

            //_geminiClient.DeclareNewFunction(
            //    name: "print_hello",
            //    description: "Prints hello in console",
            //    parameter: new Parameter()
            //    {
            //        Type = SchemaType.OBJECT,
            //        Properties = new Dictionary<string, Property>()
            //        {
            //            {
            //                "name",
            //                new Property()
            //                {
            //                    Type = SchemaType.STRING,
            //                    Description = "Name of the person"
            //                }
            //            }
            //        }
            //    }
            //);


            _geminiClient.ListenToContentReceived(OnResponseReceived);
            _geminiClient.ListenToConnected(OnConnected);

            await _geminiClient.Connect(model, _context);
        }

        private async void OnDestroy()
        {
            await _geminiClient.Disconnect();
        }

        protected virtual void DeclareFunctions() { }

        protected FunctionDeclaration GetFunctionDeclaration(string methodName)
        {
            var method = GetType().GetMethod(methodName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static
            );
            return FunctionAnalyzer.GetFunctionDeclaration(method);
        }

        public void EnterToolArea(ToolArea area)
        {
            _toolArea = area;
        }

        public void ExitToolArea() 
        {
            _toolArea = null;
        }

        private void onTurnComplete()
        {
            if(hasAudioAnswer)
            {
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    var audioConverter = new AudioConverter();
                    var audioClip = audioConverter.ConvertBytesToAudioClip(audioAnswer, 12000, 2); // 12000 = 24000 / 2 (rate from gemini)
                    _audioSpeaker.PlayAudioClip(audioClip);

                    audioAnswer = null;
                });

                hasAudioAnswer = false;
            }
        }

        // Send the flux audio to the server
        private void OnGetAudioFlux(float[] audioBuffer)
        {
            var audioConverter = new AudioConverter();
            var audioBase64 = audioConverter.ConvertAudioSampleToBase64String(
                audioConverter.ConvertSamplesToWav(
                    audioBuffer,
                    _audioRecorder.RecordingFrequency,
                    _audioRecorder.BufferSizeInSeconds
                )
            );

            var AudioLive = new SendMessage()
            {
                RealtimeInput = new BidiGenerateContentRealtimeInput()
                {
                    MediaChunks = new Blob[]
                    {
                        new Blob()
                        {
                            MimeType = MimeType.PCM,
                            Data = audioBase64
                        }
                    }
                }
            };


            _geminiClient.Send(AudioLive, System.Net.WebSockets.WebSocketMessageType.Binary);
        }

        private void OnConnected(string response)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                _audioRecorder.ToggleProcessing();
            });
        }


        private void OnResponseReceived(string response)
        {
            var responseObj = _geminiClient.GetResponseObject<ReceiveMessage>(response);

            if (responseObj.ServerContent != null &&
                responseObj.ServerContent.TurnComplete == true)
            {
                OnComplete?.Invoke();
                return;
            }

            if (responseObj.ServerContent != null &&
                responseObj.ServerContent.ModelTurn != null &&
                responseObj.ServerContent.ModelTurn.Parts != null &&
                responseObj.ServerContent.ModelTurn.Parts.Length > 0)
            {
                ManageServerContent(responseObj);
                return;
            }

            if (responseObj.ToolCall != null &&
                responseObj.ToolCall.FunctionCalls != null &&
                responseObj.ToolCall.FunctionCalls.Length > 0
            )
            {
                ManageFunctionCall(responseObj);
                return;
            }

            if(responseObj.ToolCallCancellation != null)
            {
                ManageFunctionCallCancelled(responseObj);
                return;
            }

        }

        private void ManageFunctionCallCancelled(ReceiveMessage responseObj)
        {
            for (int i = responseObj.ToolCallCancellation.Ids.Length - 1; i >= 0; i--)
            {
                var functionId = responseObj.ToolCallCancellation.Ids[i];
                if (_taskRunning.ContainsKey(functionId))
                {
                    _taskRunning[functionId].Cancel();
                    _taskRunning.Remove(functionId);
                }
            }
        }

        private void ManageFunctionCall(ReceiveMessage responseObj)
        {
            foreach (var functionCall in responseObj.ToolCall.FunctionCalls)
            {
                // snake_case to TitleCase
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                var functionName = textInfo.ToTitleCase(functionCall.Name.Replace("_", " "));
                functionName = functionName.Replace(" ", "");

                var method = GetType().GetMethod(functionName,
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Static
                );

                if (method != null)
                {
                    var functionDeclaration = FunctionAnalyzer.GetFunctionDeclaration(method);

                    if (functionDeclaration != null)
                    {
                        var parametersDef = functionDeclaration.Parameters.Properties;

                        var parameters = new object[parametersDef.Count];

                        int index = 0;
                        foreach (var parameter in parametersDef)
                        {
                            var value = functionCall.Args[parameter.Key];
                            parameters[index] = value;
                            index++;
                        }

                        var CancellationTokenSource = new CancellationTokenSource();

                        _taskRunning.Add(functionCall.Id, CancellationTokenSource);

                        Task.Run(() =>
                        {
                            method?.Invoke(this, parameters);
                            _taskRunning.Remove(functionCall.Id);
                        }, CancellationTokenSource.Token);
                    }
                }
            }
        }

        private void ManageServerContent(ReceiveMessage responseObj)
        {
            foreach (var part in responseObj.ServerContent.ModelTurn.Parts)
            {
                if (part.Text != null)
                {
                    Debug.Log(part.Text);
                }
                else if (part.InlineData != null)
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        var mimeTypeFull = part.InlineData.MimeType;
                        var mimeType = mimeTypeFull.Split(';')[0];

                        switch (mimeType)
                        {
                            case MimeType.PCM:
                                hasAudioAnswer = true;

                                var audioConverter = new AudioConverter();
                                int rate = int.Parse(mimeTypeFull.Split(';')[1].Split('=')[1]);
                                var bytes = audioConverter.ConvertFrom64String(part.InlineData.Data);

                                if (audioAnswer == null)
                                {
                                    audioAnswer = bytes;
                                }
                                else
                                {
                                    var newBytes = new byte[audioAnswer.Length + bytes.Length];
                                    audioAnswer.CopyTo(newBytes, 0);
                                    bytes.CopyTo(newBytes, audioAnswer.Length);
                                    audioAnswer = newBytes;
                                }
                                break;
                        }
                    });
                }
            }
        }
    }
}
