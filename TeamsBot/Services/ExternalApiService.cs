using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ExternalApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("externalApi");
        }

        public async Task<string> CallApi(BotDataRequest model)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("https://localhost:44300/get_data_of_audio_file", model);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Обработка ошибочного ответа
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                // Обработка исключения при сетевых проблемах
                Console.WriteLine($"HTTP error occurred: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Обработка других исключений
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
