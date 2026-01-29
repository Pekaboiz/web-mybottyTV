using TwitchLib.Client.Models;

namespace web_mybottyTV.Utils
{
    public sealed class UserUtils
    {
        public string ChannelId { get; init; } = default!;
        public string UserId { get; init; } = default!;
        public string UserName { get; set; } = default!;
        public int Warns { get; set; }
    }

    public readonly record struct UserKey(string ChannelId, string UserId);

    public sealed class UsersUtils
    {
        private readonly Dictionary<UserKey, UserUtils> _users = new();

        public int AddWarn(ChatMessage chat)
        {
            var user = GetOrCreateUser(chat);
            return ++user.Warns;
        }

        public int GetWarns(ChatMessage chat)
        {
            return _users.TryGetValue(
                new UserKey(chat.Channel, chat.UserId),
                out var user
            )
                ? user.Warns
                : 0;
        }

        public UserUtils GetOrCreateUser(ChatMessage chat)
        {
            var key = new UserKey(chat.Channel, chat.UserId);

            if (!_users.TryGetValue(key, out var user))
            {
                user = new UserUtils
                {
                    ChannelId = chat.Channel,
                    UserId = chat.UserId,
                    UserName = chat.Username,
                    Warns = 0
                };

                _users[key] = user;
            }
            else
            {
                user.UserName = chat.Username;
            }

            return user;
        }
    }
}
