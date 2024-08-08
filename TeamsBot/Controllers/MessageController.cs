using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

[Route("api/sendMessage")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly IBotMessageHandlerService _messageHandlerService;

    public MessageController(IBotMessageHandlerService messageHandlerService)
    {
        _messageHandlerService = messageHandlerService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MessageRequest request)
    {
        var result = await _messageHandlerService.SendMessageAsync(request.UserId, request.Message);
        if (result)
        {
            return Ok();
        }

        return NotFound();
    }
}