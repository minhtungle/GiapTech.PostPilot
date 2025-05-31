using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        public async Task<string> GetCompletionAsync(string prompt, string _apiKey)
        {
            using (var client = new HttpClient())
            {

                //var _apiKey = GetOpenAIApiKey();
                //string _apiKey = ConfigurationManager.AppSettings["OpenAI_ApiKey"];
                //string apiKey = Environment.GetEnvironmentVariable("OpenAI_ApiKey");

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiKey);

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    { new { role = "user", content = prompt } }
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