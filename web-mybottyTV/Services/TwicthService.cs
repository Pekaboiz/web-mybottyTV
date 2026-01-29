namespace web_mybottyTV.Services
{
    public class TwitchService
    {
        public string ClientId { get; }
        public string OAuthToken { get; }
        public string BotName { get; }
        public string ChannelName { get; }
        
        public TwitchService() 
        { 
            ClientId = Environment.GetEnvironmentVariable("CLIENT_ID")!;
            OAuthToken = Environment.GetEnvironmentVariable("OAUTH_TOKEN")!;
            BotName = Environment.GetEnvironmentVariable("BOT_NAME")!;
            ChannelName = Environment.GetEnvironmentVariable("TEMP_CHANNEL")!;
        }
    }
}
