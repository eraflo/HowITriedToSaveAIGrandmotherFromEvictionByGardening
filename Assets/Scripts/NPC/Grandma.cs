using Assets.Scripts.GeminiAI.Enums;
using Assets.Scripts.GeminiAI.FunctionsCall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class Grandma : NPC
    {
        protected sealed override void DeclareFunctions()
        {
            _geminiClient.DeclareNewFunction(GetFunctionDeclaration("FetchTool"));
        }

        [Function("fetch_tool", "go fetch the tool asked by the player")]
        private void FetchTool(
            [Parameter(SchemaType.STRING, "the tool to fetch")] string tool
        )
        {
            Debug.Log($"Fetching tool : {tool}");
        }
    }
}
