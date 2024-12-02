using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPTBotApp.Utilities
{
    public static class ApiHelper
    {
        public static HttpClient CreateHttpClient(string apiKey)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            return client;
        }
    }
}
