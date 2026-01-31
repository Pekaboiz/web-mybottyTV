using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using web_mybottyTV.API;
using web_mybottyTV.Services;
using static System.Net.WebRequestMethods;

namespace web_mybottyTV.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _http;
        private readonly ChannelBootstrapService _channelBootstrap;
        public AuthController(IHttpClientFactory http, ChannelBootstrapService channelBootstrap)
        {
            _http = http;
            _channelBootstrap = channelBootstrap;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var clientId = Environment.GetEnvironmentVariable("TWITCH_CLIENT_ID");
            var redirectUri = "https://localhost:5001/auth/twitch/callback";

            var url =
                "https://id.twitch.tv/oauth2/authorize" +
                $"?client_id={clientId}" +
                $"&redirect_uri={redirectUri}" +
                $"&response_type=code" +
                $"&scope=user:read:email";

            return Redirect(url);
        }

        [HttpGet("twitch/callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var clientId = Environment.GetEnvironmentVariable("TWITCH_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("TWITCH_CLIENT_SECRET");

            var http = _http.CreateClient();

            var tokenResponse = await http.PostAsync(
                "https://id.twitch.tv/oauth2/token" +
                $"?client_id={clientId}" +
                $"&client_secret={clientSecret}" +
                $"&code={code}" +
                $"&grant_type=authorization_code" +
                $"&redirect_uri=https://localhost:5001/auth/twitch/callback",
                null);

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            Console.WriteLine(tokenJson);
            var doc = JsonDocument.Parse(tokenJson);

            if (!doc.RootElement.TryGetProperty("access_token", out var tokenElement))
            {
                throw new Exception($"Twitch token error: {tokenJson}");
            }

            var token = tokenElement.GetString();

            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            http.DefaultRequestHeaders.Add("Client-Id", clientId);

            var userJson = await http.GetStringAsync("https://api.twitch.tv/helix/users");

            var user = JsonDocument.Parse(userJson)
                .RootElement.GetProperty("data")[0];

            var channel = await _channelBootstrap.EnsureChannelExistsAsync(user.GetProperty("login").GetString()!, token!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.GetProperty("id").GetString()!),
                new Claim(ClaimTypes.Name, user.GetProperty("login").GetString()!),
                new Claim("display_name", user.GetProperty("display_name").GetString()!)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Redirect("/");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }


}
