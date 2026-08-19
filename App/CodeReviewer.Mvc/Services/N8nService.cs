using System.Net.Http.Json;

namespace CodeReviewer.Mvc.Services
{
    public class N8nService
    {
        private readonly HttpClient _httpClient;

        public N8nService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendSubmissionToAIAsync(object payload)
        {
            string url = "http://localhost:5678/webhook/code-submitted";
            var response = await _httpClient.PostAsJsonAsync(url, payload);
            response.EnsureSuccessStatusCode();
        }
    }
}