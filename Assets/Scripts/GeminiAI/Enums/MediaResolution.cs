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
    public enum MediaResolution
    {
        MEDIA_RESOLUTION_UNSPECIFIED,
        MEDIA_RESOLUTION_LOW,
        MEDIA_RESOLUTION_MEDIUM,
        MEDIA_RESOLUTION_HIGH
    }
}
