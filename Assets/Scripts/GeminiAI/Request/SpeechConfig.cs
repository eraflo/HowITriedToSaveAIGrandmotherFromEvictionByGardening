using Assets.Scripts.GeminiAI.Enums;

namespace Assets.Scripts.GeminiAI.Request
{
    public class PrebuiltVoiceConfig
    {
        public PrebuiltVoice VoiceName { get; set; }
    }

    public class VoiceConfig
    {
        public PrebuiltVoiceConfig PrebuiltVoiceConfig { get; set; }
    }

    public class SpeechConfig
    {
        public VoiceConfig VoiceConfig { get; set; }
    }
}
