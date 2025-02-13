using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Request
{
    [Serializable]
    public class AIGenerationConfig
    {
        public float Temperature { get; set; }
        public int MaxOutputTokens { get; set; }
        public float TopP { get; set; }
        public float TopK { get; set; }
        public List<object> StopSequences { get; set; }
    }
}
