using Newtonsoft.Json;
using System.Net;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ExternalApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient("externalApi");
        }

        public async Task<BotDataResponse?> CallApi(BotDataRequest model)
        {
            try
            {
                string baseUrl = _configuration["ExternalApiBaseUrl"];
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("source_text", model.SourceText)
                });
                HttpResponseMessage response = await _httpClient.PostAsync($"{baseUrl}get_data_of_audio_file", formContent);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Десериализуем JSON в ApiResponseModel
                        var apiResponse = JsonConvert.DeserializeObject<BotDataResponse>(content);
                        apiResponse.Raw_string = content;

                        return apiResponse;
                    }
                }
                // Обработка ошибочного ответа
                return null;
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
