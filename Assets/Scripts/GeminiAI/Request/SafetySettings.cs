using Assets.Scripts.GeminiAI.Enums;
using System;

namespace Assets.Scripts.GeminiAI.Request
{
    [Serializable]
    public class SafetySettings
    {
        public HarmCategory Category { get; set; }
        public HarmThreshold Threshold { get; set; }
    }
}
