# HowISaveAIGrandmotherFromEvictionByGardening

## Gemini Client

You can create (please, do it in th `ScriptableObjects` folder) the Gemini client (Right Click -> Create -> Gemini -> Gemini Client).

Then, in the option, add your [api key](https://aistudio.google.com/apikey).

For the model, it's in this format : __"models/[model_name]"__ (exemple : "models/gemini-2.0-flash-exp")

For the Interaction, 2 possibilities tested for the moment :
- With __HTTP__ : use _generateContent_
- With __WebSocket__ : use _BidiGenerateContent_

For the __Response Mime Type__, seems the only one working are with the current gemini version : 
- text/plain 
- text/json 

<br/>

__Basic Values for :__ 
- `Candidate Count` : 1
- `Max Output Token` : 8192 (for flash 2.0 exp)
- `Temperature` : 1
- `Top P` : 1
- `Top K` : 1

<br/>
Don't change the Seed value, and don't use `Show Presence Penalty`, `Show Frequency Penalty`, `Activate Logprob` and `Enable Enhanced Civic Answer` as they are not supported with the current free versions of gemini models.
<br/><br/>
You can change the `Safety Settings` (Choose the level of censure of the model), the `Speech Voice Settings` (voice to use with audio) and the `Response Modalities` (TEXT : response in text format, AUDIO = response in audio format)

### Functions in Gemini Client for WebSockets (for programmers)

First thing to do is give the setup to the gemini client (You need to have a reference to a Gemini Client Scriptable Object) :

Full Example of Configuration For the Audio, with safety settings and also function calling
```c#
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
```

Then, you can use : `await _geminiClient.Connect(model);` to connect a session with the models

Also, before connecting, you can add listener to get the answers from the model :
- `_geminiClient.ListenToContentReceived(function)` -> Get content from the model
- `_geminiClient.ListenToConnected(function)` -> Get setup answer from the model

(Both listener want function with a string as a parameter)

#### SendMessage

Json Object that you can construct to send a message to the model (see gemini doc for the structure of the object : [GeminiDoc](https://ai.google.dev/gemini-api/docs/multimodal-live))

Then, you can use ___geminiClient.SendMessage() to send a message. You will the response with the listeners, so make sure to have register them. <br/><br/>

_Example :_ (Send Audio to the Model)

```c#
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
```

#### ReceiveMessage

With you listener, you get a string in argument. To get the Json Object of that string, use :
```c#
private void OnResponseReceived(string response)
{
    var responseObj = _geminiClient.GetResponseObject<ReceiveMessage>(response);
}
```

Then, for the different elements of the answer, look at the gemini documentation : [GeminiDoc](https://ai.google.dev/gemini-api/docs/multimodal-live)

## Format Audio For The Model

To send an audio file to the model, here is an example :

```c#
var audioConverter = new AudioConverter();
var audioBase64 = audioConverter.ConvertAudioSampleToBase64String(
    audioConverter.ConvertSamplesToWav(
        audioBuffer,
        [frequency],
        [bufferSizeInSeconds]
    )
);
```

You use the audio recorder object. And the right format for gemini is to convert the audio in Base64 string and then, convert it to wav.

But, for the multi modal live, __the MimeType to use is__ _(even if just before you create an audio in wav format, you need to use the following MimeType)_ : 
```c#
MimeType.PCM 
```

```c#
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
```