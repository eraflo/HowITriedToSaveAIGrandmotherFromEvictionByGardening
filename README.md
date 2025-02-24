# HowISaveAIGrandmotherFromEvictionByGardening

## Gemini Client

You can create (please, do it in th `ScriptableObjects` folder) the Gemini client (Right Click -> Create -> Gemini -> Gemini Client).

Then, in the option, add your [api key](https://aistudio.google.com/apikey).

For the url field, 2 possibilites : 

- Classic text generation with http connexion : `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent` (you can change `gemini-2.0-flash-exp` to [use another model](https://ai.google.dev/gemini-api/docs/models/gemini?hl=fr&lang=rest))
- Realtime and multi-modal features, with websocket : `wss://generativelanguage.googleapis.com/v1alpha/gemini:live`
