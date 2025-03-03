using Assets.Scripts.GeminiAI.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Request.Bidi
{
    public class BidiGenerateContentRealtimeInput
    {
        public Blob[] MediaChunks { get; set; }
    }
}
