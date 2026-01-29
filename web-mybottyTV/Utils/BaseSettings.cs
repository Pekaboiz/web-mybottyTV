using TwitchLib.Client.Models;
using TwitchLib.Client.Enums;

namespace Utils
{
    public enum TActionType
    {
        None,
        Moderation, // отвечает за мут, бан
        Stream, // смена названий стрима и игр
        Notification, // уведомления
        Chat, // реакция на сообщения в чате
    }

    public class BaseSettings
    {
        // Команда, например "!whome". Если пустая — может означать "ловить все сообщения".
        public string? CommandName { get; init; } = "!whome";

        // Тип действия — определяет обработку
        public TActionType ActionType { get; init; } = TActionType.Chat;

        // Шаблон ответа. Можно использовать плейсхолдеры: {username}, {args}
        public string? Response { get; init; } = "You are {username}!";

        // Включена ли настройка
        public bool IsEnabled { get; init; } = true;

        // Ограничение по ролям (moderator, broadcaster, subscriber и т.п.)
        public UserType AllowedRoles { get; init; } = UserType.Viewer;

        // Кулдаун в секундах между срабатываниями
        public int CooldownSeconds { get; init; } = 5;

        // Максимальное количество заглавных букв в словек
        public int CapsLimit { get; init; } = 10;

        // Время на которое человек мутится (в секундах)
        public int Duration { get; init; } = 600;

        public string Reason { get; init; } = "none";

        // Время на которое человек мутится (в секундах)
        public string? CommandExecution { get; init; } = "";

        public ChatMessage? Chat { get; set; }

        // Совпадение должно быть регистрозависимым
        public bool CaseSensitive { get; init; } = false;
    }
}
