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
    public enum BlockReason
    {
        BLOCK_REASON_UNSPECIFIED,
        SAFETY,
        OTHER,
        BLOCKLIST,
        PROHIBITED_CONTENT,
        IMAGE_SAFETY
    }
}
