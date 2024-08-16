namespace WebApplication1.Models
{
    public class BotDataRequest
    {
        public string? SessionId { get; set; }
        public string? Language { get; set; }
        public string? SourceText { get; set; }
        public byte[]? FileBytes { get; set; }
    }
}
