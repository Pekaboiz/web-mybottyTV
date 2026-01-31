using Microsoft.EntityFrameworkCore;
using Utils;
using web_mybottyTV.Data;

namespace web_mybottyTV.Services
{
    public class BotConfigLoader
    {
        private readonly AppDbContext _db;

        public BotConfigLoader(AppDbContext db)
        {
            _db = db;
        }

        public async Task<BotSettingsStorage> LoadAsync()
        {
            var channels = await _db.Channels
                .Include(c => c.Commands)
                .Where(c => c.BotEnabled)
                .ToListAsync();

            var botSettings = channels.Select(c => new BotSettings
            {
                ChannelName = c.ChannelName,
                Oauth = c.Oauth,
                MaxWarns = c.MaxWarns,
                Settings = c.Commands.Select(cmd => new BaseSettings
                {
                    CommandName = cmd.CommandName,
                    ActionType = Enum.Parse<TActionType>(cmd.ActionType.ToString()),
                    Response = cmd.Response,
                    IsEnabled = cmd.IsEnabled,
                    AllowedRoles = Enum.Parse<TwitchLib.Client.Enums.UserType>(cmd.AllowedRoles.ToString()),
                    CooldownSeconds = cmd.CooldownSeconds,
                    CapsLimit = cmd.CapsLimit,
                    Duration = cmd.Duration,
                    Reason = cmd.Reason,
                    CaseSensitive = cmd.CaseSensitive
                }).ToList()
            }).ToArray();

            return new BotSettingsStorage
            {
                BotSettings = botSettings
            };
        }
    }
}
