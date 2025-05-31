using Applications.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Applications.QuanLyAITool.Extensions
{
    public static class AIToolTemplates
    {
        public static readonly Dictionary<AIToolTypeEnum, string> RequestBodyTemplates = new Dictionary<AIToolTypeEnum, string>()
        {
            {
                AIToolTypeEnum.OpenAI_ChatGPT,
                @"{
                    ""model"": ""{{model}}"",
                    ""messages"": [
                        { ""role"": ""user"", ""content"": ""{{prompt}}"" }
                    ]
                }"
            },
            {
                AIToolTypeEnum.Google_Gemini,
                @"{
                    ""contents"": [
                        {
                            ""role"": ""user"",
                            ""parts"": [ { ""text"": ""{{prompt}}"" } ]
                        }
                    ]
                }"
            },
            {
                AIToolTypeEnum.Azure_OpenAI,
                @"{
                    ""deployment_id"": ""{{model}}"",
                    ""messages"": [
                        { ""role"": ""user"", ""content"": ""{{prompt}}"" }
                    ]
                }"
            },
            {
                AIToolTypeEnum.Anthropic_Claude,
                @"{
                    ""model"": ""{{model}}"",
                    ""prompt"": ""Human: {{prompt}}\nAssistant:"",
                    ""max_tokens_to_sample"": 300
                }"
            },
            {
                AIToolTypeEnum.Cohere,
                @"{
                    ""model"": ""{{model}}"",
                    ""prompt"": ""{{prompt}}"",
                    ""temperature"": 0.8
                }"
            },
            {
                AIToolTypeEnum.Mistral,
                @"{
                    ""model"": ""{{model}}"",
                    ""messages"": [
                        { ""role"": ""user"", ""content"": ""{{prompt}}"" }
                    ]
                }"
            }
        };

        public static string GetTemplate(AIToolTypeEnum type)
        {
            return RequestBodyTemplates.TryGetValue(type, out var template) ? template : "";
        }
        public static (bool, string) GetFormattedRequestBody_(string toolCode, string model, string prompt)
        {
            // Lấy body template theo loại công cụ
            if (!Enum.TryParse(toolCode, out AIToolTypeEnum toolTypeEnum))
            {
                return (false, $"Không xác định được loại công cụ AI: {toolCode}");
            }

            if (!AIToolTemplates.RequestBodyTemplates.TryGetValue(toolTypeEnum, out var template))
            {
                return (false, $"Chưa hỗ trợ loại công cụ: {toolCode}");
            }

            return (true, template
                .Replace("{{model}}", model ?? "")
                .Replace("{{prompt}}", prompt ?? ""));
        }
        public static (bool, string) GetFormattedRequestBody(string toolCode, string model, string prompt)
        {
            if (!Enum.TryParse<AIToolTypeEnum>(toolCode, true, out var toolType))
                return (false, $"Tên tool không hợp lệ: {toolCode}");

            object body = null;

            switch (toolType)
            {
                case AIToolTypeEnum.OpenAI_ChatGPT:
                case AIToolTypeEnum.Mistral:
                    body = new
                    {
                        model = model,
                        messages = new[] {
                    new { role = "user", content = prompt }
                }
                    };
                    break;

                case AIToolTypeEnum.Azure_OpenAI:
                    body = new
                    {
                        deployment_id = model,
                        messages = new[] {
                    new { role = "user", content = prompt }
                }
                    };
                    break;

                case AIToolTypeEnum.Google_Gemini:
                    body = new
                    {
                        contents = new[] {
                    new {
                        role = "user",
                        parts = new[] { new { text = prompt } }
                    }
                }
                    };
                    break;

                case AIToolTypeEnum.Anthropic_Claude:
                    body = new
                    {
                        model = model,
                        prompt = $"Human: {prompt}\nAssistant:",
                        max_tokens_to_sample = 300
                    };
                    break;

                case AIToolTypeEnum.Cohere:
                    body = new
                    {
                        model = model,
                        prompt = prompt,
                        temperature = 0.8
                    };
                    break;

                default:
                    return (false, $"Tool {toolType} chưa được hỗ trợ.");
            }

            return (true, JsonConvert.SerializeObject(body));
        }
    }

}