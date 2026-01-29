using Microsoft.Extensions.Options;
using TwitchLib.Api;
using Utils;
using web_mybottyTV.Service;

namespace web_mybottyTV.Utils
{
    public class ChatUtils 
    {
        private TwitchAPI _api;
        public string? BotUserId { get; internal set; }
        public string? BroadcasterId { get; internal set; }
        public Dictionary<string, string> BroadcasterIds = new(StringComparer.OrdinalIgnoreCase);

        public ChatUtils(TwitchAPI api)
        {
            _api = api;
        }

        public void GetIdByLogin(string login) => BroadcasterId = BroadcasterIds[login] ?? null;

        private async void SetIdByLogin(string login)
        {
            if (BroadcasterIds.ContainsKey(login))
                return;

            try
            {
                var users = await _api.Helix.Users.GetUsersAsync(logins: new List<string> { login });
                var id = users.Users.FirstOrDefault()?.Id;
                if (!string.IsNullOrEmpty(id))
                {
                    BroadcasterIds[login] = id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async void GetBroadcasters(IOptionsMonitor<BotSettingsStorage> botSettingsMonitor)
        {
            try
            {
                var cfgs = botSettingsMonitor.CurrentValue?.BotSettings ?? Array.Empty<BotSettings>();
                foreach (var cfg in cfgs)
                {
                    var login = cfg.ChannelName;

                    if (!string.IsNullOrWhiteSpace(login))
                        SetIdByLogin(login);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }

}        
