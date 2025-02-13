using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Request
{
    public enum PrebuiltVoice
    {

    }

    public class PrebuiltVoiceConfig
    {
        public string VoiceName { get; set; }
    }

    public class SpeechConfig
    {
        public PrebuiltVoiceConfig PrebuiltVoiceConfig { get; set; }
    }
}
