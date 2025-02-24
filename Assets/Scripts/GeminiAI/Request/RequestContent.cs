using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Request
{
    public class RequestContent
    {
        public string Role { get; set; }
        public RequestPart[] Parts { get; set; }
    }
}
