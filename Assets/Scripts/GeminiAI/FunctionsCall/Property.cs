using Assets.Scripts.GeminiAI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.FunctionsCall
{
    public class Property
    {
        public SchemaType Type { get; set; }
        public string Description { get; set; }

        // Only for type "enum"
        public string[] Values { get; set; }
    }
}
