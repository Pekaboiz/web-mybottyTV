using dotenv.net;
using Utils;
using web_mybottyTV.API;
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

            DotEnv.Load();

            builder.Services.AddDbContextFactory<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<ChannelBootstrapService>();

            builder.Services.AddControllers();
            builder.Services.AddRazorPages();
            builder.Services.AddOpenApi();

            builder.Services.AddHttpClient<TwitchApiClient>(client =>
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

            builder.Services.AddSingleton<ChannelConfigProvider>();
            builder.Services.AddSingleton<TwitchService>();
            builder.Services.AddSingleton<BotSettingsService>();

            builder.Services.AddHostedService<TwitchBotHostedService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

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
