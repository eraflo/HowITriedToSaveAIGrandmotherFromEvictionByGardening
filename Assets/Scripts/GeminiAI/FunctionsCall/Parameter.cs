using Assets.Scripts.GeminiAI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.FunctionsCall
{
    public class Parameter
    {
        public SchemaType Type { get; set; }
        public Dictionary<string, Property> Properties { get; set; }
        public string[] Required { get; set; }
    }
}
