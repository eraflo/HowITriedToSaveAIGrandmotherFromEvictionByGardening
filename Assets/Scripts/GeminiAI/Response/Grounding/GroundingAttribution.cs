using Assets.Scripts.GeminiAI.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response.Grounding
{
    public class GroundingAttribution
    {
        public AttributionSourceId SourceId { get; set; }
        public Content Content { get; set; }
    }
}
