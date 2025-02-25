using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response.LogProb
{
    public class LogprobsResult
    {
        public TopCandidates[] TopCandidates { get; set; }
        public Candidate[] ChosenCandidates { get; set; }
    }
}
