using Microsoft.AspNetCore.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using Microsoft.Bot.Schema;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddControllers();

// Настройка адаптера без аутентификации для локального тестирования
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();
builder.Services.AddSingleton<BotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();

// Регистрация хранилища и состояний
builder.Services.AddSingleton<IStorage, MemoryStorage>();
builder.Services.AddSingleton<UserState>();
builder.Services.AddSingleton<ConversationState>();

// Регистрация ConcurrentDictionary для хранения ConversationReference
builder.Services.AddSingleton<ConcurrentDictionary<string, ConversationReference>>();

// Регистрация HttpClient
builder.Services.AddHttpClient();

// Регистрация бота и BotServices
builder.Services.AddTransient<IBot, TeamsBot>();
builder.Services.AddSingleton<BotServices>();

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