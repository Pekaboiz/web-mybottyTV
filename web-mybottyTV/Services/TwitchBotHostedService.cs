using Microsoft.Extensions.Options;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using Utils;
using web_mybottyTV.Services;
using web_mybottyTV.Utils;

namespace web_mybottyTV.Service
{
    public class TwitchBotHostedService : BackgroundService
    {
        private readonly TwitchService _service;
        private readonly ILogger<TwitchBotHostedService> _logger;
        private readonly IOptionsMonitor<BotSettingsStorage> _botSettingsMonitor;
        private TwitchClient _client = new TwitchClient();
        private ChatService _chat = new ChatService();

        public TwitchBotHostedService(
            TwitchService service,
            ILogger<TwitchBotHostedService> logger,
            IOptionsMonitor<BotSettingsStorage> botSettingsMonitor)
        {
            _service = service;
            _logger = logger;
            _botSettingsMonitor = botSettingsMonitor;

            _botSettingsMonitor.OnChange(cfg => _logger.LogInformation("BotSettings changed"));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _chat = new ChatService();
            _chat.Initialize(_service, _logger, _botSettingsMonitor);

            _client = _chat.Client;

            _client.OnConnected += (_, e) => OnConnected(_, e);
            _client.OnMessageReceived += (_, e) => OnMessageReceived(_, e);
            _client.OnJoinedChannel += (_, e) => OnJoinedChannel(_, e);
            
            await _client.ConnectAsync();
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"Joined channel: {e.Channel}");
            await _client!.SendMessageAsync(e.Channel, "I'm here FeelsOkayMan");
        }

        private async Task OnConnected(object? sender, OnConnectedEventArgs e)
        {
            _logger.LogInformation("Connected as {Bot}", e.BotUsername);
            await _client!.JoinChannelAsync("#"+_service.ChannelName);
        }

        private async Task OnMessageReceived(object? sender, OnMessageReceivedArgs e)
        {
            try
            {
                if (_client is null) return;

                if (string.IsNullOrEmpty(e.ChatMessage.Channel) || string.IsNullOrEmpty(e.ChatMessage.Message))
                    return;

                string? channelName = e.ChatMessage.Channel;

                _chat.ChatData.GetIdByLogin(e.ChatMessage.Channel);

                _chat.GetMySettings(channelName);

                if (_chat.BotSettings is null) return;

                _chat.API.Settings.AccessToken = _chat.BotSettings.Oauth;

                _logger.LogInformation("{User}: {Message}",
                     e.ChatMessage.Username,
                     e.ChatMessage.Message);
                
                _chat.User.GetOrCreateUser(e.ChatMessage);

                _ = HandleChatMassageAsync(e.ChatMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in OnMessageReceived");
            }
        }

        private async Task HandleChatMassageAsync(ChatMessage chatMsg)
        { 
            string? message = chatMsg.Message;
            string? channelName = chatMsg.Channel;

            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(channelName))
                return;

            if (ChatHandler.IsCommand(message))
            {
                _chat.GetConfig(chatMsg);
                _chat.ChatConfig.Chat = chatMsg;

                if (!_chat.IsUserHavePermission(chatMsg.UserType))
                {
                    message = ChatHandler.FindPlaceholder("{username}, you dont have permission to use this command", _chat) ?? "You don't have permissions";
                    await _client.SendMessageAsync(channelName, message);
                    return;
                }

               await _client.SendMessageAsync(channelName, await ChatHandler.Response(_chat, chatMsg.Message));
            }
            else
            {
                _chat.GetConfig("#CAPS_CHECK#");
                _chat.ChatConfig.Chat = chatMsg;

                if (chatMsg.UserType == UserType.Viewer && ChatHandler.IsCapsViolation(message, _chat.ChatConfig.CapsLimit))
                {
                    await ChatHandler.WarnHandler(_chat);
                    await _client.SendMessageAsync(channelName, await ChatHandler.Response(_chat, chatMsg.Message));
                }
            }
        }
    }
}
