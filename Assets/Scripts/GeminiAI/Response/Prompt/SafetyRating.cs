using Assets.Scripts.GeminiAI.Enums;
using Assets.Scripts.GeminiAI.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response.Prompt
{
    public class SafetyRating
    {
        public HarmCategory Category { get; set; }
        public HarmProbability Probability { get; set; }
        public bool Blocked { get; set; }
    }
}
