using Utils;

public class BotSettingsService
{
    private readonly Dictionary<string, BotSettings> _channels =
        new(StringComparer.OrdinalIgnoreCase);

    public void ReplaceAll(IEnumerable<BotSettings> channels)
    {
        _channels.Clear();

        foreach (var channel in channels)
        {
            _channels[channel.ChannelName] = channel;
        }
    }

    public void Upsert(BotSettings channel)
    {
        _channels[channel.ChannelName] = channel;
    }

    public BotSettings? GetChannel(string channel)
    {
        _channels.TryGetValue(channel, out var settings);
        return settings;
    }

    public IReadOnlyCollection<BotSettings> GetAllChannels() => _channels.Values;

    public BaseSettings? GetSettings(string channel, string command)
    {
        if (!_channels.TryGetValue(channel, out var ch))
            return null;

        return ch.Settings.FirstOrDefault(c =>
            c.CommandName.Equals(command, StringComparison.OrdinalIgnoreCase));
    }
}
