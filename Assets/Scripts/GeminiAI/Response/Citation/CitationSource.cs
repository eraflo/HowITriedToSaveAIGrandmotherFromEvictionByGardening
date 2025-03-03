using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Response.Citation
{
    public class CitationSource
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string Uri { get; set; }
        public string License { get; set; }
    }
}
