using System;
using System.Text.RegularExpressions;
using TwitchLib.Api;
using TwitchLib.Api.Helix;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using TwitchLib.Api.Helix.Models.Moderation.BanUser;
using TwitchLib.Client.Models;
using Utils;
using web_mybottyTV.Service;

namespace web_mybottyTV.Utils
{
    public static class ChatHandler
    {
        private static ChatService? _chat = null;
        private static readonly Regex PlaceholderRegex = new(@"\{(?<key>[^{}]+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private static readonly IReadOnlyDictionary<string, Func<ChatService, string?>> PlaceholderGetters =
            new Dictionary<string, Func<ChatService, string?>>(StringComparer.OrdinalIgnoreCase)
            {
                ["username"] = c => "@" + c.ChatConfig.Chat.Username ?? "incorrect",
                ["warn"] = c => c.User.GetWarns(c.ChatConfig.Chat).ToString(),
                ["maxwarn"] = c => (c.BotSettings.MaxWarns).ToString(),
            };

        public static bool IsCommand(string msg) => msg.StartsWith("!") ? true : false; 

        public static string FindPlaceholder(string msg, ChatService args)
        {
            if (msg is null) return string.Empty;
            if (args is null) return msg;

            if (!msg.Contains('{')) return msg;

            var result = PlaceholderRegex.Replace(msg, m =>
            {
                var key = m.Groups["key"].Value.Trim();
                if (key.Length == 0) return m.Value;

                if (PlaceholderGetters.TryGetValue(key, out var getter))
                {
                    var val = getter(args) ?? string.Empty;
                    return val;
                }

                return m.Value;
            });

            return result;
        }

        public static bool IsCapsViolation(string msg, int capsLimit)
        {
            if (string.IsNullOrWhiteSpace(msg) || capsLimit <= 0)
                return false;

            var words = msg.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                int upperCnt = 0;
                int letterCnt = 0;

                foreach (var item in word)
                {
                    if (char.IsLetter(item))
                    {
                        letterCnt++;
                        if (char.IsUpper(item))
                            upperCnt++;
                    }
                }

                if (letterCnt > 0 && upperCnt > capsLimit)
                {
                    return true;
                }
            }

            return false;
        }

        internal static async Task<string> Response(ChatService chat, string msg)
        {
            string message = string.Empty;
            _chat = chat;

            if (string.IsNullOrEmpty(_chat.ChatConfig.Response)) return string.Empty;

            switch (_chat.ChatConfig.ActionType)
            {
                case TActionType.None:
                    break;
                case TActionType.Moderation:
                    message = ChatHandler.FindPlaceholder(_chat.ChatConfig.Response, _chat);
                    break;
                case TActionType.Stream:
                    if (_chat.ChatConfig.CommandName == "!title" || _chat.ChatConfig.CommandName == "!game")
                    {
                        var modifyChannel = GetModification(msg);
                        await SetTitle(modifyChannel);
                        _chat.log.LogInformation("Channel information updated via chat command.");
                    }

                    message = ChatHandler.FindPlaceholder(_chat.ChatConfig.Response, _chat);
                    break;
                case TActionType.Notification:
                    break;
                case TActionType.Chat:
                    message = ChatHandler.FindPlaceholder(_chat.ChatConfig.Response, _chat);
                    break;
            }

            return message;
        }

        internal static ModifyChannelInformationRequest GetModification(string msg)
        {
            string newTitle = string.Empty;
            string newGame = string.Empty;

            switch (_chat.ChatConfig.CommandName)
            {
                case "!title":
                    newTitle = msg.Replace(_chat.ChatConfig.CommandName ?? string.Empty, "", StringComparison.OrdinalIgnoreCase).Trim();
                    break;
                case "!game":
                    newGame = msg.Replace(_chat.ChatConfig.CommandName ?? string.Empty, "", StringComparison.OrdinalIgnoreCase).Trim();
                    break;
            };

            return new ModifyChannelInformationRequest
            {
                Title = newTitle,
                GameId = newGame
            };
        }

        static async Task SetTitle(ModifyChannelInformationRequest modifyChannel)
        {
            await _chat.API.Helix.Channels.ModifyChannelInformationAsync(_chat.BroadcasterId, modifyChannel);
        }

        internal static async Task BanUser(ChatService chat, BaseSettings chatConfig)
        {
            try
            {
                await chat.BotAPI!.Helix.Moderation.BanUserAsync(
                    broadcasterId: chat.BroadcasterId,
                    moderatorId: chat.BotUserId,
                    banUserRequest: new BanUserRequest
                    {
                        UserId = chatConfig.Chat.UserId,
                        Duration = chatConfig.Duration > 0 ? chatConfig.Duration : null,
                        Reason = chatConfig.Reason
                    }
                );
            }
            catch (Exception ex) { chat.log.LogInformation(ex.Message); }
        }

        internal static async Task DeleteMsg(ChatService chat, BaseSettings chatConfig)
        {
            try
            {
                await chat.BotAPI!.Helix.Moderation.DeleteChatMessagesAsync(
                    broadcasterId: chat.BroadcasterId,
                    moderatorId: chat.BotUserId,
                    messageId: chatConfig.Chat.Id
                );
            }
            catch (Exception ex) { chat.log.LogInformation(ex.Message); }
        }

        internal static async Task WarnHandler(ChatService chat)
        {
            chat.User.AddWarn(chat.ChatConfig.Chat);
            chat.log.LogInformation("{username} get {count} warns", chat.ChatConfig.Chat.Username, chat.User.GetWarns(chat.ChatConfig.Chat));
            if (chat.User.GetWarns(chat.ChatConfig.Chat) == chat.BotSettings.MaxWarns)
            {
                await ChatHandler.BanUser(chat, chat.ChatConfig);
                chat.User.GetOrCreateUser(chat.ChatConfig.Chat).Warns = chat.BotSettings.MaxWarns;
            }
            else
            {
                await ChatHandler.DeleteMsg(chat, chat.ChatConfig);
            }
        }
    }
}
