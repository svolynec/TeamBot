using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class TeamsBot : ActivityHandler
{
    // private readonly BotServices _botServices;
    private readonly DialogSet _dialogs;
    private readonly UserState _userState;
    private readonly ConversationState _conversationState;

    public TeamsBot(UserState userState, ConversationState conversationState)
    {
        // _botServices = botServices;
        _userState = userState;
        _conversationState = conversationState;

        // Создание набора диалогов
        _dialogs = new DialogSet(_conversationState.CreateProperty<DialogState>("DialogState"));
        _dialogs.Add(new WaterfallDialog("mainDialog", new WaterfallStep[]
        {
            Step1Async,
            Step2Async
        }));
    }

    private async Task<DialogTurnResult> Step1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        // Логика первого шага диалога
        await stepContext.Context.SendActivityAsync("Это первый шаг диалога.", cancellationToken: cancellationToken);
        return await stepContext.NextAsync(null, cancellationToken);
    }

    private async Task<DialogTurnResult> Step2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        // Логика второго шага диалога
        await stepContext.Context.SendActivityAsync("Это второй шаг диалога.", cancellationToken: cancellationToken);
        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        var userInput = turnContext.Activity.Text;

        // Отправка сообщения
        await turnContext.SendActivityAsync(MessageFactory.Text($"Вы сказали: {userInput}"), cancellationToken);

        // Пересылка user input в API
        // var responseFromApi = await _botServices.SendToApi(userInput);
        await turnContext.SendActivityAsync(MessageFactory.Text($"Ответ от API: responseFromApi"), cancellationToken);

        // Получение состояния диалогов
        var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

        // Запуск или продолжение диалога
        var result = await dialogContext.ContinueDialogAsync(cancellationToken);
        if (result.Status == DialogTurnStatus.Empty)
        {
            await dialogContext.BeginDialogAsync("mainDialog", null, cancellationToken);
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
