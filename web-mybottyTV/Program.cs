using dotenv.net;
using Utils;
using web_mybottyTV.API;
using web_mybottyTV.Service;
using web_mybottyTV.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using web_mybottyTV.Data;

namespace web_mybottyTV
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Загружаем .env ДО DI
            DotEnv.Load();

            builder.Services.AddDbContext<AppDbContext>(options =>
                 options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<ChannelBootstrapService>();

            builder.Services.AddControllers();
            builder.Services.AddRazorPages();   
            builder.Services.AddOpenApi();

            builder.Services.AddHttpClient("internal-api", client =>
            {
                client.BaseAddress = new Uri(
                    Environment.GetEnvironmentVariable("BASE_API_URL")!
                );
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
            });

            // Привязываем настройки бота и регистрируем в DI
            builder.Configuration.AddJsonFile("config/botsettings.json", optional: true, reloadOnChange: true);
            builder.Services.Configure<BotSettingsStorage>(builder.Configuration.GetSection("BotSettingsStorage"));

            // Регистрируем Twitch
            builder.Services.AddSingleton<TwitchApiClient>();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();

            app.Run();
        }
    }
}
