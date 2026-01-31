using Utils;

namespace web_mybottyTV.API
{
    public class TwitchApiClient
    {
        private readonly HttpClient _http;

        public TwitchApiClient(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("internal-api");
        }

        public async Task<BotSettings[]> GetAllUsersAsync()
        {
            return await _http.GetFromJsonAsync<BotSettings[]>($"twitch/users")
                ?? throw new InvalidOperationException("Empty response");
        }

        public async Task<BotSettings> GetUsersAsync(string login)
        {
            return await _http.GetFromJsonAsync<BotSettings>($"twitch/users?login={Uri.EscapeDataString(login)}") 
                ?? throw new InvalidOperationException("Empty response");
        }

        public async Task<BaseSettings> GetSettingsAsync(string login, string cmd)
        {
            return await _http.GetFromJsonAsync<BaseSettings>($"twitch/users?login={Uri.EscapeDataString(login)}&command={cmd}")
                ?? throw new InvalidOperationException("Empty response");
        }
    }
}
