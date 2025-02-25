using Assets.Scripts.GeminiAI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Request
{
    [Serializable]
    public class GenerationConfig
    {
        public string[] StopSequences { get; set; }
        public string ResponseMimeType { get; set; }
        public Schema ResponseSchema { get; set; }
        public ResponseModality[] ResponseModalities { get; set; }
        public int CandidateCount { get; set; }
        public int MaxOutputTokens { get; set; }
        public float Temperature { get; set; }
        public float TopP { get; set; }
        public int TopK { get; set; }
        public int Seed { get; set; }
        public float? PresencePenalty { get; set; }
        public float? FrequencyPenalty { get; set; }
        public bool? ResponseLogprobs { get; set; } = null;
        public int? Logprobs { get; set; } = null;
        public bool EnableEnhancedCivicAnswers { get; set; }
        public SpeechConfig SpeechConfig { get; set; }
        public MediaResolution MediaResolution { get; set; }
    }
}
