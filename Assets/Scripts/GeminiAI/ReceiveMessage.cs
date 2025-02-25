using Assets.Scripts.GeminiAI.Response.Bidi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI
{
    public class ReceiveMessage
    {
        public BidiGenerateContentSetupComplete SetupComplete { get; set; }
        public BidiGenerateContentServerContent ServerContent { get; set; }
        public BidiGenerateContentToolCall ToolCall { get; set; }
        public BidiGenerateContentToolCallCancellation ToolCallCancellation { get; set; }
    }
}
