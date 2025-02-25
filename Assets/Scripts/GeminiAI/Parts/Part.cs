using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GeminiAI.Parts
{
    public class Part
    {
        public string Text { get; set; }
        public Blob InlineData { get; set; }
        public FunctionCall FunctionCall { get; set; }
        public FunctionResponse FunctionResponse { get; set; }
        public FileData FileData { get; set; }
        public ExecutableCode ExecutableCode { get; set; }
        public CodeExecutionResult CodeExecutionResult { get; set; }
    }
}
