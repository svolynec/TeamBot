using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Models
{
    public class Action
    {
        public string name { get; set; }
        public bool executed { get; set; }
        public ActionResult result { get; set; }
    }
}
