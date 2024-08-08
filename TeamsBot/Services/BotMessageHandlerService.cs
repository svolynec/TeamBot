using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using System.Collections.Concurrent;

namespace WebApplication1.Services
{
    public class BotMessageHandlerService : IBotMessageHandlerService
    {
        private readonly BotFrameworkHttpAdapter _adapter;
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

        public BotMessageHandlerService(BotFrameworkHttpAdapter adapter, ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            _adapter = adapter;
            _conversationReferences = conversationReferences;
        }

        public async Task<bool> SendMessageAsync(string userId, string message)
        {
            if (_conversationReferences.TryGetValue(userId, out ConversationReference reference))
            {
                await _adapter.ContinueConversationAsync(string.Empty, reference, async (context, token) =>
                {
                    await context.SendActivityAsync(MessageFactory.Text(message), token);
                }, default);
                return true;
            }

            return false;
        }
    }
}
