using Assets.Scripts.GeminiAI.Response.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response.Grounding
{
    public class GroundingMetadata
    {
        public GroundingChunk[] GroundingChunks { get; set; }
        public GroundingSupport[] GroundingSupports { get; set; }
        public string[] WebSearchQueries { get; set; }
        public SearchEntryPoint SearchEntryPoint { get; set; }
        public RetrievalMetadata RetrievalMetadata { get; set; }
    }
}
