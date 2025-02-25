using Assets.Scripts.GeminiAI.Enums;

namespace Assets.Scripts.GeminiAI.Response.Prompt
{
    public class PromptFeedback
    {
        public BlockReason BlockReason { get; set; }
        public SafetyRating[] SafetyRatings { get; set; }
    }
}
