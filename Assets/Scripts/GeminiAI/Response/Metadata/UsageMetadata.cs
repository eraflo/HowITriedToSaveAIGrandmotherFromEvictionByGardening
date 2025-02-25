using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response.Metadata
{
    public class UsageMetadata
    {
        public int PromptTokenCount { get; set; }
        public int CachedContentTokenCount { get; set; }
        public int CandidatesTokenCount { get; set; }
        public int ToolUsePromptTokenCount { get; set; }
        public int TotalTokenCount { get; set; }
        public ModalityTokenCount[] PromptTokensDetails { get; set; }
        public ModalityTokenCount[] CacheContentTokensDetails { get; set; }
        public ModalityTokenCount[] CandidatesTokensDetails { get; set; }
        public ModalityTokenCount[] ToolUsePromptTokensDetails { get; set; }
    }
}
