using System.ComponentModel.DataAnnotations;

namespace InfoSymbolServer.Application.Dtos;

/// <summary>
/// Data transfer object for updating notification settings
/// </summary>
public record UpdateNotificationSettingsDto
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
