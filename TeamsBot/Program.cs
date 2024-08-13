using Microsoft.AspNetCore.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using Microsoft.Bot.Schema;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddControllers();

// Настройка адаптера без аутентификации для локального тестирования
builder.Services.AddSingleton<IBotMessageHandlerService, BotMessageHandlerService>();

builder.Services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<BotFrameworkHttpAdapter>>();
    
    var credentialProvider = new SimpleCredentialProvider(
        configuration["MicrosoftAppId"],
        configuration["MicrosoftAppPassword"]);
    
    return new BotFrameworkHttpAdapter(credentialProvider);
});

builder.Services.AddSingleton<BotFrameworkHttpAdapter, BotFrameworkHttpAdapter>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<BotFrameworkHttpAdapter>>();
    
    var credentialProvider = new SimpleCredentialProvider(
        configuration["MicrosoftAppId"],
        configuration["MicrosoftAppPassword"]);
    
    return new BotFrameworkHttpAdapter(credentialProvider);
});

// Регистрация хранилища и состояний
builder.Services.AddSingleton<IStorage, MemoryStorage>();
builder.Services.AddSingleton<UserState>();
builder.Services.AddSingleton<ConversationState>();

// Регистрация ConcurrentDictionary для хранения ConversationReference
var conversationReferences = new ConcurrentDictionary<string, ConversationReference>();
builder.Services.AddSingleton(conversationReferences);

// Регистрация HttpClient
builder.Services.AddHttpClient();

// Регистрация бота и BotServices
builder.Services.AddTransient<IBot, TeamsBot>();
builder.Services.AddSingleton<BotServices>();

builder.Services.AddSingleton<ExternalApiService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.Map("/api/messages", async context =>
    {
        var adapter = context.RequestServices.GetRequiredService<IBotFrameworkHttpAdapter>();
        var bot = context.RequestServices.GetRequiredService<IBot>();
        await adapter.ProcessAsync(context.Request, context.Response, bot);
    });
});

app.Run();