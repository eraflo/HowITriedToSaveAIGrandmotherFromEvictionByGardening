using Assets.Scripts.GeminiAI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Parts
{
    public class CodeExecutionResult
    {
        public Outcome Outcome { get; set; }
        public string Output { get; set; }
    }
}
