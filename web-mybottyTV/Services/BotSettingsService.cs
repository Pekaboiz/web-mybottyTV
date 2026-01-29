using Microsoft.Extensions.Options;
using System.Text.Json;
using Utils;
using web_mybottyTV.Utils;

public class BotSettingsService
{
    private readonly string _filePath = "config/botsettings.json";
    private BotSettingsStorage _storage = new BotSettingsStorage();

    public BotSettingsService(IOptions<BotSettingsStorage> options)
    {
        _storage = options.Value;
    }

    public BotSettings GetChannel(string username) => _storage.BotSettings
                                                                .FirstOrDefault(bs => bs.ChannelName.Equals(username, StringComparison.OrdinalIgnoreCase))
                                                                ?? new BotSettings();

    public BotSettings[] GetChannel() => _storage.BotSettings;

    public void Save()
    {
        var json = JsonSerializer.Serialize(
            _storage,
            new JsonSerializerOptions { WriteIndented = true }
        );

        File.WriteAllText(_filePath, json);
    }
}
