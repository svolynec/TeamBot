namespace WebApplication1.Models
{
    public class CommandItem
    {
        public string Name { get; set; }
        public string Language { get; set; }

        public List<object> Params { get; set; }
        public List<Action> Actions { get; set; }
    }

    public class CommandContainer
    {
        public CommandItem Command { get; set; }
    }
}