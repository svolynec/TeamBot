namespace WebApplication1.Models
{
    public class BotDataResponse
    {
        public string Session_id { get; set; }
        public string Language { get; set; }
        public string Source_text { get; set; }
        public List<Command> Commands { get; set; }
        public object Error_message { get; set; }
        public object Error_code { get; set; }
        public string? Raw_string { get; set; }
    }
}
