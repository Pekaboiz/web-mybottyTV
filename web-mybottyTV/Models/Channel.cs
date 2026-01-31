using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TwitchLib.Client.Enums;
using Utils;

namespace web_mybottyTV.Models
{
    public class Channel
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("channel_name")]
        public string ChannelName { get; set; } = string.Empty;

        [Column("oauth_token")]
        public string Oauth { get; set; } = string.Empty;
        
        [Column("max_warns")]
        [System.ComponentModel.DefaultValue(3)]
        public int MaxWarns { get; set; } = 3;

        [Column("bot_enabled")]
        public bool BotEnabled { get; set; } = true;
        public virtual ICollection<Command> Commands { get; set; } = new HashSet<Command>();
    }

    public class Command
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("channel_id")]
        public Guid ChannelId { get; set; }

        [Required]
        [Column("command_name")]
        public string CommandName { get; set; } = string.Empty;

        [Required]
        [Column("action_type")]
        public TActionType ActionType { get; set; } = TActionType.Chat;
        [Column("response")]
        public string? Response { get; set; }

        [Required]
        [Column("is_enabled")]
        public bool IsEnabled { get; set; } = true;

        [Column("allowed_roles")]
        public UserType AllowedRoles { get; set; } = UserType.Viewer;

        [Column("cooldown_seconds")]
        public int CooldownSeconds { get; set; } = 5;

        [Column("caps_limit")]
        public int CapsLimit { get; set; } = 10;
        
        [Column("duration")]
        public int Duration { get; set; } = 600;
        
        [Column("reason")]
        public string? Reason { get; set; } = null;

        [Column("case_sensitive")]
        public bool CaseSensitive { get; set; } = false;

        [ForeignKey(nameof(ChannelId))]
        public Channel Channel { get; set; } = null!;
    }
}
