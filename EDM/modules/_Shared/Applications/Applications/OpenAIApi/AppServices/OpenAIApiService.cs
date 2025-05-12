using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Applications.OpenAIApi.AppServices
{
    public class OpenAIApiService
    {
        //private readonly string _apiKey = "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";
        private static readonly string _privateConfigPath = HttpContext.Current.Server.MapPath("~/App_Data/appsettings.Private.json");
        public static string GetOpenAIApiKey()
        {
            if (!File.Exists(_privateConfigPath))
                return Environment.GetEnvironmentVariable("OpenAI_ApiKey");

            var json = File.ReadAllText(_privateConfigPath);
            dynamic config = JsonConvert.DeserializeObject(json);
            return config?.OpenAI?.ApiKey;
        }
        public async Task<string> GetCompletionAsync(string prompt)
        {
            using (var client = new HttpClient())
            {

                var _apiKey = GetOpenAIApiKey();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiKey);

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                    new { role = "user", content = prompt }
                }
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return $"Lỗi từ OpenAI: {response.StatusCode} - {responseString}";
                };
                dynamic json = JsonConvert.DeserializeObject(responseString);
                if (json?.choices != null && json.choices.Count > 0)
                {
                    return (string)json.choices[0].message.content;
                }
                else
                {
                    return "Không nhận được nội dung từ OpenAI.";
                }
            }
        }
    }
}