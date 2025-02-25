using Assets.Scripts.GeminiAI.Request.Bidi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI
{
    public class SendMessage
    {
        public BidiGenerateContentSetup Setup { get; set; }
        public BidiGenerateContentClientContent ClientContent { get; set; }
        public BidiGenerateContentRealtimeInput RealtimeInput { get; set; }
        public BidiGenerateContentToolResponse ToolResponse { get; set; }
    }
}
