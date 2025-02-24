using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Request
{
    public enum PrebuiltVoice
    {
        None,
        Puck,
        Charon,
        Kore,
        Fenrir,
        Aoede
    }

    public class PrebuiltVoiceConfig
    {
        public string VoiceName { get; set; }
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
