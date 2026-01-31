using Utils;
using TwitchLib.Api;
using TwitchLib.Client;
using web_mybottyTV.API;
using web_mybottyTV.Utils;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;
using Microsoft.Extensions.Options;

namespace web_mybottyTV.Services
{
    public class ChatService
    {
        
        private TwitchAPI _api;
        private TwitchAPI _apiBroadcaster;
        private TwitchClient _client;
        private TwitchService _service;
        private ChatUtils _chatUtils;
        private ILogger<TwitchBotHostedService> _logger;

        private bool _initialized;

        private BotSettingsStorage _botStorage;
        private BotSettings? _botSettings;
        private BaseSettings? _config;
        private UsersUtils _usersUtils = new UsersUtils();

        private TwitchApiClient _twitchApi;

        public ChatService(TwitchApiClient twitchApi)
        {
            _client = new TwitchClient();
            _api = new TwitchAPI();
            _apiBroadcaster = new TwitchAPI();
            _twitchApi = twitchApi;
        }

        public void Initialize(TwitchService service, ILogger<TwitchBotHostedService> logger, BotSettingsStorage botStorage)
        {
            if (_initialized)
                throw new InvalidOperationException("ChatService уже инициализирован");

            _logger = logger;
            _service = service;
            _botStorage = botStorage;

            var credentials = new ConnectionCredentials(
                _service.BotName,
                "oauth:" + _service.OAuthToken
            );

            _client.Initialize(credentials);

            _api.Settings.ClientId = _service.ClientId;
            _api.Settings.AccessToken = _service.OAuthToken;

            _apiBroadcaster.Settings.ClientId = _service.ClientId;

            SetDataChat();

            _initialized = true;
        }

        private async void SetDataChat()
        {
            if (_chatUtils is null)
            {
                _chatUtils = new ChatUtils(_api);
            }

            if (_botStorage is not null)
            {
                _chatUtils.GetBroadcasters(_botStorage);
            }

            try
            {
                var botUsers = await _api.Helix.Users.GetUsersAsync(logins: new List<string> { _service.BotName });
                _chatUtils.BotUserId = botUsers.Users.FirstOrDefault()?.Id;
                //_chatUtils.GetIdByLogin(_service.ChannelName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve bot user id at startup.");
            }
        }

        public async Task<BotSettings>? GetMySettings(string channelName)
        {
            _botSettings = await _twitchApi.GetUsersAsync(channelName);
            
            return _botSettings;
        }

        public async Task<BotSettings[]>? GetAll()
        {
            return await _twitchApi.GetAllUsersAsync();
        }

        public async Task<BaseSettings>? GetConfig(ChatMessage e)
        {
            if (_botSettings is null)
                return null;

            string command = e.Message.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

            _config = await _twitchApi.GetSettingsAsync(e.Channel, command);

            return _config;
        }

        public async Task<BaseSettings> GetConfig(string command)
        {
            if (_botSettings == null)
                return null;

            _config = await _twitchApi.GetSettingsAsync(_botSettings.ChannelName!, command);

            return _config ?? null;
        }

        public bool IsUserHavePermission(UserType userType)
        {
            if ((int)userType < (int)ChatConfig.AllowedRoles)
                return false;
            else return true;
        }

        private T ThrowError<T>() => throw new InvalidOperationException("ChatService не инициализирован");

        public TwitchClient Client => _initialized ? _client : ThrowError<TwitchClient>();
        public ChatUtils ChatData => _initialized ? _chatUtils : ThrowError<ChatUtils>();
        public BotSettings? BotSettings => _initialized ? _botSettings : ThrowError<BotSettings>();
        public BaseSettings ChatConfig => _initialized ? (_config ?? new BaseSettings()) : ThrowError<BaseSettings>();
        public TwitchAPI API => _initialized ? _apiBroadcaster : ThrowError<TwitchAPI>();
        public TwitchAPI BotAPI => _initialized ? _api : ThrowError<TwitchAPI>();
        public UsersUtils User => _initialized ? _usersUtils : ThrowError<UsersUtils>();
        public ILogger<TwitchBotHostedService> log => _initialized ? _logger : ThrowError<ILogger<TwitchBotHostedService>>();
        public string? BotUserId => _initialized ? _chatUtils.BotUserId : ThrowError<string>();
        public string? BroadcasterId => _initialized ?  _chatUtils.BroadcasterId : ThrowError<string>();
        public string BotName => _initialized ? _service.BotName : ThrowError<string>();
    }
}
