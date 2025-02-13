using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GeminiAI.Request
{
    public class AIRequest
    {
        public RequestContent[] Contents { get; set; }
        public AIGenerationConfig GenerationConfig { get; set; }
        public SafetySettings[] SafetySettings { get; set; }
    }
}
