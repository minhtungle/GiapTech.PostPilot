using Applications.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static string GetFormattedRequestBody(string toolName, string model, string prompt)
        {
            if (Enum.TryParse<AIToolTypeEnum>(toolName, ignoreCase: true, out var toolType))
            {
                if (RequestBodyTemplates.TryGetValue(toolType, out var template))
                {
                    return string.Format(template, model, prompt);
                }
                else
                {
                    return $"Không tìm thấy template cho tool '{toolType}'";
                }
            }
            else
            {
                return $"Tên tool không hợp lệ: {toolName}";
            }
        }
    }

}