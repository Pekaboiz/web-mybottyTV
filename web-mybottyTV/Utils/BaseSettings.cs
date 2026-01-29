using TwitchLib.Client.Models;
using TwitchLib.Client.Enums;

namespace Utils
{
    public enum TActionType
    {
        None,
        Moderation, 
        Stream, 
        Notification, 
        Chat, 
    }

    public class BaseSettings
    {
        public string? CommandName { get; init; } = "!whome";
        public TActionType ActionType { get; init; } = TActionType.Chat;
        public string? Response { get; init; } = "You are {username}!";
        public bool IsEnabled { get; init; } = true;
        public UserType AllowedRoles { get; init; } = UserType.Viewer;
        public int CooldownSeconds { get; init; } = 5;
        public int CapsLimit { get; init; } = 10;
        public int Duration { get; init; } = 600;
        public string Reason { get; init; } = "none";
        public string? CommandExecution { get; init; } = "";
        public ChatMessage? Chat { get; set; }
        public bool CaseSensitive { get; init; } = false;
    }
}
