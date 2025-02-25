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
    public enum FinishReason
    {
        FINISH_REASON_UNSPECIFIED,
        STOP,
        MAX_TOKENS,
        SAFETY,
        RECITATION,
        LANGUAGE,
        OTHER,
        BLOCKLIST,
        PROHIBITED_CONTENT,
        SPII,
        MALFORMED_FUNCTION_CALL,
        IMAGE_SAFETY
    }
}
