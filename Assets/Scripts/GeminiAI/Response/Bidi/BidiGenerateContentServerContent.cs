using Assets.Scripts.GeminiAI.Parts;
using Assets.Scripts.GeminiAI.Response.Grounding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response.Bidi
{
    public class BidiGenerateContentServerContent
    {
        public bool TurnComplete { get; set; }
        public bool Interrupted { get; set; }
        public GroundingMetadata GroundingMetadata { get; set; }
        public Content ModelTurn { get; set; }
    }
}
