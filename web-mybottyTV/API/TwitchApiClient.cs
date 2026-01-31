using Utils;

namespace web_mybottyTV.API
{
    public class TwitchApiClient
    {
        private readonly HttpClient _http;

        public TwitchApiClient(HttpClient http)
        {
            _http = http;
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

        internal async Task PushUserAsync(string id, BotSettings config)
        {
            var response = await _http.PostAsJsonAsync($"twitch/users?login={id}", config);

            response.EnsureSuccessStatusCode();
        }
    }
}
