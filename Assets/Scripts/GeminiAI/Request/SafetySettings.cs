using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Request
{
    public enum SafetyThreshold
    {
        BLOCK_NONE,
        BLOCK_ONLY_HIGH,
        BLOCK_MEDIUM_AND_ABOVE,
        BLOCK_LOW_AND_ABOVE,
        HARM_BLOCK_THRESHOLD_UNSPECIFIED
    }

    public enum SafetyCategory
    {
        HARM_CATEGORY_HARASSMENT,
        HARM_CATEGORY_HATE_SPEECH,
        HARM_CATEGORY_SEXUALLY_EXPLICIT,
        HARM_CATEGORY_DANGEROUS_CONTENT,
        HARM_CATEGORY_CIVIC_INTEGRITY
    }

    [Serializable]
    public class SafetySettings
    {
        public string Category { get; set; }
        public string Threshold { get; set; }
    }
}
