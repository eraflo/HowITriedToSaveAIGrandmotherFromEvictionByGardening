using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response
{
    public class Candidate
    {
        public Content Content { get; set; }
        public string FinishReason { get; set; }
        public int Index { get; set; }
        public SafetyRating[] SafetyRatings { get; set; }
    }
}
