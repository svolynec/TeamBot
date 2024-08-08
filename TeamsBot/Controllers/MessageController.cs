using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WebApplication1.Models;

[Route("api/sendMessage")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly BotFrameworkHttpAdapter  _adapter;
    private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

    public MessageController(BotFrameworkHttpAdapter  adapter, ConcurrentDictionary<string, ConversationReference> conversationReferences)
    {
        _adapter = adapter;
        _conversationReferences = conversationReferences;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MessageRequest request)
    {
        if (_conversationReferences.TryGetValue(request.UserId, out ConversationReference reference))
        {
            await _adapter.ContinueConversationAsync(string.Empty, reference, async (context, token) =>
            {
                await context.SendActivityAsync(MessageFactory.Text(request.Message), token);
            }, default);
            return Ok();
        }

        return NotFound();
    }
}