using Assets.Scripts.GeminiAI.Enums;
using Assets.Scripts.GeminiAI.Request;
using Assets.Scripts.GeminiAI.Parts;
using Assets.Scripts.Utils;
using System.Collections.Generic;

namespace Assets.Scripts.GeminiAI
{
    public class APIRequestFactory
    {
        public static AIRequest CreateRequest(string prompt)
        {
            return new AIRequest
            {
                Contents = new Content[]
                {
                    new Content
                    {
                        Role = Role.User,
                        Parts = new Part[]
                        {
                            new Part
                            {
                                Text = prompt
                            }
                        }
                    }
                },
                GenerationConfig = new GenerationConfig
                {
                    MaxOutputTokens = 2048,
                    Temperature = 1,
                    TopP = 1,
                    TopK = 1,
                    StopSequences = new string[] {  }
                },
                SafetySettings = new SafetySettings[]
                {
                    new SafetySettings
                    {
                        Category = HarmCategory.HARM_CATEGORY_HARASSMENT,
                        Threshold = HarmThreshold.BLOCK_ONLY_HIGH
                    },
                    new SafetySettings
                    {
                        Category = HarmCategory.HARM_CATEGORY_HATE_SPEECH,
                        Threshold = HarmThreshold.BLOCK_ONLY_HIGH
                    },
                    new SafetySettings
                    {
                        Category = HarmCategory.HARM_CATEGORY_SEXUALLY_EXPLICIT,
                        Threshold = HarmThreshold.BLOCK_ONLY_HIGH
                    },
                    new SafetySettings
                    {
                        Category = HarmCategory.HARM_CATEGORY_DANGEROUS_CONTENT,
                        Threshold = HarmThreshold.BLOCK_ONLY_HIGH
                    }
                }
            };
        }

        public static AIRequest CreateRequest(
            string prompt, 
            Part[] parts = null,
            string model = null,
            GenerationConfig generationConfig = null,
            SafetySettings[] safetySettings = null
        ) {
            return new AIRequest
            {
                Contents = new Content[]
                {
                    new Content
                    {
                        Role = Role.User,
                        Parts = new Part[]
                        {
                            new Part
                            {
                                Text = prompt
                            }
                        }
                    }
                },
                Model = model,
                GenerationConfig = generationConfig,
                SafetySettings = safetySettings
            };
        }
    }
}