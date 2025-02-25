using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HarmThreshold
    {
        OFF,
        BLOCK_NONE,
        BLOCK_ONLY_HIGH,
        BLOCK_MEDIUM_AND_ABOVE,
        BLOCK_LOW_AND_ABOVE,
        HARM_BLOCK_THRESHOLD_UNSPECIFIED
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum HarmCategory
    {
        HARM_CATEGORY_HARASSMENT,
        HARM_CATEGORY_HATE_SPEECH,
        HARM_CATEGORY_SEXUALLY_EXPLICIT,
        HARM_CATEGORY_DANGEROUS_CONTENT,
        HARM_CATEGORY_CIVIC_INTEGRITY
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum HarmProbability
    {
        HARM_PROBABILITY_UNSPECIFIED,
        NEGLIGIBLE,
        LOW,
        MEDIUM,
        HIGH
    }
}
