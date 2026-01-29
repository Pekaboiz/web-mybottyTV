using System;
using dotenv.net;
using web_mybottyTV.Twitch;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Utils;

namespace web_mybottyTV
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Загружаем .env ДО DI
            DotEnv.Load();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // Привязываем настройки бота и регистрируем в DI
            builder.Configuration.AddJsonFile("config/botsettings.json", optional: true, reloadOnChange: true);
            builder.Services.Configure<BotSettingsStorage>(builder.Configuration.GetSection("BotSettingsStorage"));

            // Регистрируем Twitch
            builder.Services.AddSingleton<TwitchService>();
            builder.Services.AddHostedService<TwitchBotHostedService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
