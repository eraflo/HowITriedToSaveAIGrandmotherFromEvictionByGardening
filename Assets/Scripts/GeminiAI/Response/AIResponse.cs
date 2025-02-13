using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response
{
    public class AIResponse
    {
        public Candidate[] Candidates { get; set; }
        public PromptFeedback PromptFeedback { get; set; }
    }
}
