using Assets.Scripts.GeminiAI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Request
{
    public class Schema
    {
        public SchemaType Type { get; set; }
        public string Format { get; set; }
        public string Description { get; set; }
        public bool Nullable { get; set; }
        public string[] Enum { get; set; }
        public string MaxItems { get; set; }
        public string MinItems { get; set; }
        public Dictionary<string, Schema> Properties { get; set; }
        public string[] Required { get; set; }
        public string[] PropertyOrdering { get; set; }
        public Schema Items { get; set; }
    }
}
