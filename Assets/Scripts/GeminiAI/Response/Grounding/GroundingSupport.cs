using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response.Grounding
{
    public class GroundingSupport
    {
        public int[] GroundingChunkIndices { get; set; }
        public float[] ConfidenceScores { get; set; }
        public Segment Segment { get; set; }
    }
}
