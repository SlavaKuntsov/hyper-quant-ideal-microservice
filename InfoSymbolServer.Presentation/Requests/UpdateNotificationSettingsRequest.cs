using System.ComponentModel.DataAnnotations;

namespace InfoSymbolServer.Presentation.Requests;

/// <summary>
/// Request model for updating notification settings
/// </summary>
public record UpdateNotificationSettingsRequest
{
    /// <summary>
    /// Gets or sets whether Telegram notifications are enabled
    /// </summary>
    public bool IsTelegramEnabled { get; init; }
    
    /// <summary>
    /// Gets or sets whether Email notifications are enabled
    /// </summary>
    public bool IsEmailEnabled { get; init; }
} 