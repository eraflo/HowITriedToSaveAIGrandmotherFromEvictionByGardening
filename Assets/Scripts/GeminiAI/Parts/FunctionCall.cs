using Assets.Scripts.GeminiAI.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Parts
{
    public class FunctionCall
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Args { get; set; }
    }
}
