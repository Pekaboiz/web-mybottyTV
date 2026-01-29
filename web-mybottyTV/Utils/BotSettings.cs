using System.Collections.Generic;

namespace Utils
{
    public class BotSettingsStorage
    {
        public BotSettings[] BotSettings { get; init; } = System.Array.Empty<BotSettings>();
    }

    public class BotSettings
    {
        public string? ChannelName { get; init; } = "#xxx";
        public string? Oauth { get; init; } = "xxx";

        public int MaxWarns { get; init; } = 3;

        public List<BaseSettings> Settings { get; init; } = new();
    }
}
