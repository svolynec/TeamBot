using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Services;

public class TeamsBot : ActivityHandler
{
    private readonly DialogSet _dialogs;
    private readonly UserState _userState;
    private readonly ExternalApiService _externalApiService;
    private readonly ConversationState _conversationState;
    private readonly IConfiguration _configuration;
    private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

    public TeamsBot(IConfiguration configuration, UserState userState, ConversationState conversationState,
        ConcurrentDictionary<string, ConversationReference> conversationReferences,
        ExternalApiService externalApiService)
    {
        _userState = userState;
        _conversationState = conversationState;
        _conversationReferences = conversationReferences;
        _externalApiService = externalApiService;
        _configuration = configuration;

        // Создание набора диалогов
        _dialogs = new DialogSet(_conversationState.CreateProperty<DialogState>("DialogState"));
        _dialogs.Add(new WaterfallDialog("mainDialog", new WaterfallStep[]
        {
            Step1Async,
        }));
    }

    private async Task<DialogTurnResult> Step1Async(WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        // Логика второго шага диалога
        // await stepContext.Context.SendActivityAsync("UserId: " + stepContext.Context.Activity.From.Id,
        //     cancellationToken: cancellationToken);
        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,
        CancellationToken cancellationToken)
    {
        var userInput = turnContext.Activity.Text;

        // Сохранение ConversationReference
        var conversationReference = turnContext.Activity.GetConversationReference();
        _conversationReferences.AddOrUpdate(turnContext.Activity.From.Id, conversationReference,
            (key, newValue) => conversationReference);

        // Проверка на наличие вложений (аудио)
        if (turnContext.Activity.Attachments is { Count: > 0 })
        {
            //foreach (var attachment in turnContext.Activity.Attachments)
            //{
            //    if (attachment.ContentType == "audio/wav" || attachment.ContentType == "audio/mpeg")
            //    {
            //        // Обработка аудио-вложения
            //        var audioUrl = attachment.ContentUrl;
            //        await turnContext.SendActivityAsync(MessageFactory.Text($"Получено аудио: {audioUrl}"), cancellationToken);
            //    }
            //}

            var response = await _externalApiService.CallApi(new WebApplication1.Models.BotDataRequest
            {
                SourceText = userInput,
                Language = null,
                FileBytes = null,
                SessionId = turnContext.Activity.From.Id
            });

            if (response != null)
            {
                var mode = _configuration["Mode"];
                if (mode == "debug")
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Язык: {response.Language}"),
                        cancellationToken);
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Язык: {response.Source_text}"),
                        cancellationToken);
                }

                foreach (var command in response.Commands)
                {
                    if (command.Command.Actions.Count <= 0) continue;
                    foreach (var action in command.Command.Actions)
                    {
                        if (action.result?.Output != null)
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text($"Ответ: {action.result.Output}"),
                                cancellationToken);
                        }
                    }
                }

                if (mode == "debug")
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"{response.Raw_string}"),
                        cancellationToken);
                }
            }

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

    protected override Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
        ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        if (membersAdded.Any(member => member.Id != turnContext.Activity.Recipient.Id))
        {
            var welcomeText = "Добро пожаловать в Teams Bot!";
            return turnContext.SendActivityAsync(MessageFactory.Text(welcomeText), cancellationToken);
        }

        return base.OnMembersAddedAsync(membersAdded, turnContext, cancellationToken);
    }
}