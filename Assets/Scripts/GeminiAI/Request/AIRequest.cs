using Assets.Scripts.GeminiAI.Parts;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GeminiAI.Request
{
    public class AIRequest
    {
        public string Model { get; set; }
        public Content[] Contents { get; set; }
        public GenerationConfig GenerationConfig { get; set; }
        public SafetySettings[] SafetySettings { get; set; }
    }
}
