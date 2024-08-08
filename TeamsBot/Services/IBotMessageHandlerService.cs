namespace WebApplication1.Services
{
    public interface IBotMessageHandlerService
    {
        Task<bool> SendMessageAsync(string userId, string message);
    }
}
