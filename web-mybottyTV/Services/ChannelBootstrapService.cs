using Microsoft.EntityFrameworkCore;
using web_mybottyTV.Data;
using web_mybottyTV.Models;

namespace web_mybottyTV.Services
{
    public class ChannelBootstrapService
    {
        private readonly AppDbContext _dbContext;

        public ChannelBootstrapService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Channel> EnsureChannelExistsAsync(string channelName, string token)
        {
            var channel = await _dbContext.Channels
                .FirstOrDefaultAsync(c => c.ChannelName.ToLower() == channelName.ToLower());
            if (channel == null)
            {
                channel = new Channel
                {
                    ChannelName = channelName,
                    Oauth = token,
                    MaxWarns = 3,
                    BotEnabled = true
                };

                channel.Commands.Add(new Command
                {
                    CommandName = "!hello",
                    Response = "Hello, welcome to the channel!",
                    IsEnabled = true
                });

                _dbContext.Channels.Add(channel);
                await _dbContext.SaveChangesAsync();
            }

            return channel;
        }
    }
}
