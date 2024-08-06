using Microsoft.AspNetCore.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddControllers();

// Настройка аутентификации
var botFrameworkAuthentication = new ConfigurationBotFrameworkAuthentication(builder.Configuration);

// Настройка адаптера
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, CloudAdapter>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<CloudAdapter>>();
    return new CloudAdapter(botFrameworkAuthentication, logger);
});

// Регистрация хранилища и состояний
builder.Services.AddSingleton<IStorage, MemoryStorage>();
builder.Services.AddSingleton<UserState>();
builder.Services.AddSingleton<ConversationState>();

// Регистрация бота
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
app.MapControllers();

app.Run();