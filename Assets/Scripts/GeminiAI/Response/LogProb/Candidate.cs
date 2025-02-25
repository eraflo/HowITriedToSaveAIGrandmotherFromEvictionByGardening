using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response.LogProb
{
    public class Candidate
    {
        public string Token { get; set; }
        public int TokenId { get; set; }
        public float LogProbability { get; set; }
    }
}
