using Assets.Scripts.GeminiAI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.FunctionsCall
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class ParameterAttribute : Attribute
    {
        public SchemaType Type { get; }
        public string Description { get; }
        public string[] Values { get; }

        public ParameterAttribute(SchemaType type, string description, params string[] values)
        {
            Type = type;
            Description = description;

            if(values != null && values.Length > 0)
            {
                Values = values;
            }
        }
    }
}
