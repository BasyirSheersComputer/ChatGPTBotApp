using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatGPTBotApp.Services
{
    public interface IChatService
    {
        Task<string> SendMessageAsync(string message);
    }
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ChatService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<string> SendMessageAsync(string message)
        {
            var payload = new
            {
                model = "gpt-4o-2024-11-20",
                messages = new[] { new { role = "user", content = message } }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _apiKey) },
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseJson = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return responseJson.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}
