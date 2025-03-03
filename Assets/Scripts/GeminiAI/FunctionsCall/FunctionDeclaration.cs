using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.FunctionsCall
{
    public class FunctionDeclaration
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Parameter Parameters { get; set; }
    }
}
