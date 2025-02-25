using Assets.Scripts.GeminiAI.FunctionsCall;
using Assets.Scripts.GeminiAI.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Request.Bidi
{
    public class BidiGenerateContentSetup
    {
        public string Model { get; set; }
        public GenerationConfig GenerationConfig { get; set; }
        public Content SystemInstruction { get; set; }
        public Tool[] Tools { get; set; }
    }
}
