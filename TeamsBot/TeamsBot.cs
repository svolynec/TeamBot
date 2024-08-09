using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class TeamsBot : ActivityHandler
{
    private readonly DialogSet _dialogs;
    private readonly UserState _userState;
    private readonly ConversationState _conversationState;
    private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

    public TeamsBot(UserState userState, ConversationState conversationState, ConcurrentDictionary<string, ConversationReference> conversationReferences)
    {
        _userState = userState;
        _conversationState = conversationState;
        _conversationReferences = conversationReferences;

        // Создание набора диалогов
        _dialogs = new DialogSet(_conversationState.CreateProperty<DialogState>("DialogState"));
        _dialogs.Add(new WaterfallDialog("mainDialog", new WaterfallStep[]
        {
            Step1Async,
        }));
    }

    private async Task<DialogTurnResult> Step1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        // Логика второго шага диалога
        await stepContext.Context.SendActivityAsync("UserId: " + stepContext.Context.Activity.From.Id, cancellationToken: cancellationToken);
        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        var userInput = turnContext.Activity.Text;
        
        // Сохранение ConversationReference
        var conversationReference = turnContext.Activity.GetConversationReference();
        _conversationReferences.AddOrUpdate(turnContext.Activity.From.Id, conversationReference, (key, newValue) => conversationReference);

        // Проверка на наличие вложений (аудио)
        if (turnContext.Activity.Attachments != null && turnContext.Activity.Attachments.Count > 0)
        {
            foreach (var attachment in turnContext.Activity.Attachments)
            {
                if (attachment.ContentType == "audio/wav" || attachment.ContentType == "audio/mpeg")
                {
                    // Обработка аудио-вложения
                    var audioUrl = attachment.ContentUrl;
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Получено аудио: {audioUrl}"), cancellationToken);
                }
            }
        }
        else
        {
            // Отправка сообщения
            await turnContext.SendActivityAsync(MessageFactory.Text($"Вы сказали: {userInput}"), cancellationToken);

            // Получение состояния диалогов
            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

            // Запуск или продолжение диалога
            var result = await dialogContext.ContinueDialogAsync(cancellationToken);
            if (result.Status == DialogTurnStatus.Empty)
            {
                await dialogContext.BeginDialogAsync("mainDialog", null, cancellationToken);
            }
        }
    }

    protected override Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        foreach (var member in membersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                var welcomeText = "Добро пожаловать в Teams Bot!";
                return turnContext.SendActivityAsync(MessageFactory.Text(welcomeText), cancellationToken);
            }
        }

        return base.OnMembersAddedAsync(membersAdded, turnContext, cancellationToken);
    }
}
