using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Request
{
    public enum Modality
    {
        TEXT,
        AUDIO
    }

    public class ResponseModality
    {
        public string Modality { get; set; }
    }
}
