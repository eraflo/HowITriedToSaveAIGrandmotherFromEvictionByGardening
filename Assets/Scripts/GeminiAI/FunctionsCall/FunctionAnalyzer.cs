using Assets.Scripts.GeminiAI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GeminiAI.FunctionsCall
{
    public static class FunctionAnalyzer
    {
        public static FunctionDeclaration GetFunctionDeclaration(MethodInfo method)
        {
            var functionAttribute = method.GetCustomAttribute<FunctionAttribute>();
            if (functionAttribute == null)
            {
                return null;
            }

            var functionDeclaration = new FunctionDeclaration
            {
                Name = functionAttribute.Name,
                Description = functionAttribute.Description,
                Parameters = null
            };


            foreach (var parameter in method.GetParameters())
            {
                var parameterAttribute = parameter.GetCustomAttribute<ParameterAttribute>();
                if (parameterAttribute != null)
                {
                    if(functionDeclaration.Parameters == null)
                    {
                        functionDeclaration.Parameters = new Parameter()
                        {
                            Type = SchemaType.OBJECT,
                            Properties = new Dictionary<string, Property>()
                        };
                    }

                    var property = new Property
                    {
                        Type = parameterAttribute.Type,
                        Description = parameterAttribute.Description,
                        Values = null
                    };

                    if(parameterAttribute.Values != null)
                    {
                        Debug.Log("Values : " + parameterAttribute.Values);
                        property.Values = parameterAttribute.Values;
                    }

                    functionDeclaration.Parameters.Properties[parameter.Name] = property;
                }
            }

            return functionDeclaration;
        }
    }
}
