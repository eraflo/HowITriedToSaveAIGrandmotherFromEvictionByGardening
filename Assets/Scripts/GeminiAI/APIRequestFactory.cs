using Assets.Scripts.GeminiAI.Request;
using Assets.Scripts.GeminiAI.Request.RequestParts;
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
                Contents = new RequestContent[]
                {
                    new RequestContent
                    {
                        Role = "user",
                        Parts = new RequestPart[]
                        {
                            new TextPart
                            {
                                Text = prompt
                            }
                        }
                    }
                },
                GenerationConfig = new AIGenerationConfig
                {
                    MaxOutputTokens = 2048,
                    Temperature = 1,
                    TopP = 1,
                    TopK = 1,
                    StopSequences = new List<object>()
                },
                SafetySettings = new SafetySettings[]
                {
                    new SafetySettings
                    {
                        Category = "HARM_CATEGORY_HARASSMENT",
                        Threshold = EnumUtils.GetEnumFieldToString(SafetyThreshold.BLOCK_ONLY_HIGH)
                    },
                    new SafetySettings
                    {
                        Category = "HARM_CATEGORY_HATE_SPEECH",
                        Threshold = EnumUtils.GetEnumFieldToString(SafetyThreshold.BLOCK_ONLY_HIGH)
                    },
                    new SafetySettings
                    {
                        Category = "HARM_CATEGORY_SEXUALLY_EXPLICIT",
                        Threshold = EnumUtils.GetEnumFieldToString(SafetyThreshold.BLOCK_ONLY_HIGH)
                    },
                    new SafetySettings
                    {
                        Category = "HARM_CATEGORY_DANGEROUS_CONTENT",
                        Threshold = EnumUtils.GetEnumFieldToString(SafetyThreshold.BLOCK_ONLY_HIGH)
                    }
                }
            };
        }

        public static AIRequest CreateRequest(
            string prompt,
            AIGenerationConfig generationConfig = null
        )
        {
            return new AIRequest
            {
                Contents = new RequestContent[]
                {
                    new RequestContent
                    {
                        Role = "user",
                        Parts = new RequestPart[]
                        {
                            new TextPart
                            {
                                Text = prompt
                            }
                        }
                    }
                },
                GenerationConfig = generationConfig,
                SafetySettings = new SafetySettings[]
                {
                    new SafetySettings
                    {
                        Category = "HARM_CATEGORY_HARASSMENT",
                        Threshold = EnumUtils.GetEnumFieldToString(SafetyThreshold.BLOCK_ONLY_HIGH)
                    },
                    new SafetySettings
                    {
                        Category = "HARM_CATEGORY_HATE_SPEECH",
                        Threshold = EnumUtils.GetEnumFieldToString(SafetyThreshold.BLOCK_ONLY_HIGH)
                    },
                    new SafetySettings
                    {
                        Category = "HARM_CATEGORY_SEXUALLY_EXPLICIT",
                        Threshold = EnumUtils.GetEnumFieldToString(SafetyThreshold.BLOCK_ONLY_HIGH)
                    },
                    new SafetySettings
                    {
                        Category = "HARM_CATEGORY_DANGEROUS_CONTENT",
                        Threshold = EnumUtils.GetEnumFieldToString(SafetyThreshold.BLOCK_ONLY_HIGH)
                    }
                }
            };
        }

        public static AIRequest CreateRequest(
            string prompt,
            SafetySettings[] safetySettings = null
        )
        {
            return new AIRequest
            {
                Contents = new RequestContent[]
                {
                    new RequestContent
                    {
                        Role = "user",
                        Parts = new RequestPart[]
                        {
                            new TextPart
                            {
                                Text = prompt
                            }
                        }
                    }
                },
                GenerationConfig = new AIGenerationConfig
                {
                    MaxOutputTokens = 2048,
                    Temperature = 1,
                    TopP = 1,
                    TopK = 1,
                    StopSequences = new List<object>()
                },
                SafetySettings = safetySettings
            };
        }

        public static AIRequest CreateRequest(
            string prompt, 
            AIGenerationConfig generationConfig = null,
            SafetySettings[] safetySettings = null
        ) {
            return new AIRequest
            {
                Contents = new RequestContent[]
                {
                    new RequestContent
                    {
                        Role = "user",
                        Parts = new RequestPart[]
                        {
                            new TextPart
                            {
                                Text = prompt
                            }
                        }
                    }
                },
                GenerationConfig = generationConfig,
                SafetySettings = safetySettings
            };
        }
    }
}