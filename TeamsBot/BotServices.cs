using System.Net.Http;
using System.Threading.Tasks;

public class BotServices
{
    private readonly HttpClient _httpClient;

    public BotServices(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> SendToApi(string userInput)
    {
        var response = await _httpClient.PostAsync("https://your-api-endpoint", new StringContent(userInput));
        return await response.Content.ReadAsStringAsync();
    }
}