using dotenv.net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Utils;
using web_mybottyTV.API;
using web_mybottyTV.Service;
using web_mybottyTV.Services;

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
            builder.Services.AddRazorPages();   
            builder.Services.AddOpenApi();

            builder.Services.AddHttpClient<TwitchApiClient>(client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("BASE_API_URL")!);
            });

            // Привязываем настройки бота и регистрируем в DI
            builder.Configuration.AddJsonFile("config/botsettings.json", optional: true, reloadOnChange: true);
            builder.Services.Configure<BotSettingsStorage>(builder.Configuration.GetSection("BotSettingsStorage"));

            // Регистрируем Twitch
            builder.Services.AddSingleton<TwitchService>();
            builder.Services.AddHostedService<TwitchBotHostedService>();
            builder.Services.AddSingleton<BotSettingsService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            //app.UseHttpsRedirection(); // SSL
            app.UseStaticFiles();
            app.UseRouting();
            app.MapRazorPages();    
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
