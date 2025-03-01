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

namespace Assets.Scripts.NPC
{
    [RequireComponent(typeof(AudioRecorder), typeof(AudioSpeaker))]
    public class NPC : MonoBehaviour
    {
        [Header("Gemini Client")]
        [SerializeField] private GeminiClient _geminiClient;
        [SerializeField] private string model;
        [SerializeField] private PrebuiltVoice _voice;

        private AudioRecorder _audioRecorder;
        private AudioSpeaker _audioSpeaker;

        private byte[] audioAnswer;
        private bool hasAudioAnswer = false;

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


            _geminiClient.DeclareNewFunction(
                name: "print_hello",
                description: "Prints hello in console",
                parameter: new Parameter()
                {
                    Type = SchemaType.OBJECT,
                    Properties = new Dictionary<string, Property>()
                    {
                        {
                            "name",
                            new Property()
                            {
                                Type = SchemaType.STRING,
                                Description = "Name of the person"
                            }
                        }
                    }
                }
            );


            _geminiClient.ListenToContentReceived(OnResponseReceived);
            _geminiClient.ListenToConnected(OnConnected);

            await _geminiClient.Connect(model);
        }

        private void onTurnComplete()
        {
            if(hasAudioAnswer)
            {
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    var audioConverter = new AudioConverter();
                    var audioClip = audioConverter.ConvertBytesToAudioClip(audioAnswer, 12000, 2);
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

            if(responseObj.ServerContent.TurnComplete == true)
            {
                OnComplete?.Invoke();
                return;
            }

            if (responseObj.ServerContent != null &&
                responseObj.ServerContent.ModelTurn != null &&
                responseObj.ServerContent.ModelTurn.Parts != null &&
                responseObj.ServerContent.ModelTurn.Parts.Length > 0)
            {
                foreach (var part in responseObj.ServerContent.ModelTurn.Parts)
                {
                    if (part.Text != null)
                    {
                        Debug.Log(part.Text);
                    }
                    else if(part.InlineData != null)
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

                                    if(audioAnswer == null)
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
}
