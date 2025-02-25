using Assets.Scripts.GeminiAI.Enums;
using Assets.Scripts.GeminiAI.Response.Citation;
using Assets.Scripts.GeminiAI.Response.Grounding;
using Assets.Scripts.GeminiAI.Response.LogProb;
using Assets.Scripts.GeminiAI.Response.Prompt;
using Assets.Scripts.GeminiAI.Parts;

namespace Assets.Scripts.GeminiAI.Response
{
    public class Candidat
    {
        public Content Content { get; set; }
        public FinishReason FinishReason { get; set; }
        public SafetyRating[] SafetyRatings { get; set; }
        public CitationMetadata CitationMetadata { get; set; }
        public int TokenCount { get; set; }
        public GroundingAttribution[] GroundingAttributions { get; set; }
        public GroundingMetadata GroundingMetadata { get; set; }
        public float AvgLogprobs { get; set; }
        public LogprobsResult LogprobsResult { get; set; }
        public int Index { get; set; }
    }
}
