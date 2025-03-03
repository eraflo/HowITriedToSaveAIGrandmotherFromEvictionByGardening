using Assets.Scripts.GeminiAI.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Request.Bidi
{
    public class BidiGenerateContentClientContent
    {
        public Content[] Turns { get; set; }
        public bool TurnComplete { get; set; }
    }
}
