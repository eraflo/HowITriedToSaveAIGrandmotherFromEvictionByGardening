using Assets.Scripts.GeminiAI.Response.Metadata;
using Assets.Scripts.GeminiAI.Response.Prompt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response
{
    public class GenerateContentResponse
    {
        public Candidat[] Candidates { get; set; }
        public PromptFeedback PromptFeedback { get; set; }
        public UsageMetadata UsageMetadata { get; set; }
        public string ModelVersion { get; set; }
    }
}
