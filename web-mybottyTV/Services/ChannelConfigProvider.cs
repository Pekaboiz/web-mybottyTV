using Microsoft.EntityFrameworkCore;
using System.Linq;
using Utils;
using web_mybottyTV.API;
using web_mybottyTV.Data;
using web_mybottyTV.Models;

namespace web_mybottyTV.Services
{
    public class ChannelConfigProvider
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly TwitchApiClient _api;

        public ChannelConfigProvider(IDbContextFactory<AppDbContext> dbFactory, TwitchApiClient api)
        {
            _dbFactory = dbFactory;
            _api = api;
        }

        public async Task<BotSettings> ReloadChannelAsync(string channelId)
        {
            await using var dbContext = _dbFactory.CreateDbContext();

            var channel = await dbContext.Channels
                .Include(c => c.Commands)
                .SingleOrDefaultAsync(c => c.ChannelName == channelId);

            if (channel == null)
                throw new ArgumentException("Channel not found", nameof(channelId));

            var config = new BotSettings
            {
                ChannelName = channel.ChannelName,
                Oauth = channel.Oauth,
                MaxWarns = channel.MaxWarns,
                Settings = channel.Commands.Select(c => new BaseSettings
                {
                    CommandName = c.CommandName,
                    Response = c.Response,
                    IsEnabled = c.IsEnabled,
                    CooldownSeconds = c.CooldownSeconds
                }).ToList()
            };

            await _api.PushUserAsync(channel.ChannelName, config);
            return config;
        }

        public async Task<BotSettingsStorage> ReloadAllChannelAsync()
        {
            await using var dbContext = _dbFactory.CreateDbContext();

            var channels = await dbContext.Channels
                .Include(c => c.Commands)
                .ToListAsync(); // channels имеет тип List<Channel>

            BotSettingsStorage storage = new BotSettingsStorage();

            storage.BotSettings = channels
                .Select<Channel, BotSettings>(MapChannelToBotSettings)
                .ToArray();

            return storage;
        }

        // Вспомогательный метод для конвертации Channel -> BotSettings
        private BotSettings MapChannelToBotSettings(Channel channel)
        {
            var settings = new BotSettings
            {
                ChannelName = channel.ChannelName,
                Oauth = channel.Oauth,
                MaxWarns = channel.MaxWarns,
                Settings = channel.Commands.Select(c => new BaseSettings
                {
                    CommandName = c.CommandName,
                    Response = c.Response,
                    IsEnabled = c.IsEnabled,
                    CooldownSeconds = c.CooldownSeconds,
                    CapsLimit = c.CapsLimit,
                    Duration = c.Duration,
                    Reason = c.Reason,
                    AllowedRoles = c.AllowedRoles,
                    CaseSensitive = c.CaseSensitive,
                }).ToList()
            };

            return settings;
        }
    }
}
