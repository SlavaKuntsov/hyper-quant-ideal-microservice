namespace InfoSymbolServer.Application.Dtos;

/// <summary>
/// Data transfer object for notification settings
/// </summary>
public record NotificationSettingsDto
{
    /// <summary>
    /// Gets or sets whether Telegram notifications are enabled
    /// </summary>
    public bool IsTelegramEnabled { get; init; }
    
    /// <summary>
    /// Gets or sets whether Email notifications are enabled
    /// </summary>
    public bool IsEmailEnabled { get; init; }
    
    /// <summary>
    /// Gets or sets whether Telegram notifications are properly configured
    /// </summary>
    public bool IsTelegramConfigured { get; init; }
    
    /// <summary>
    /// Gets or sets whether Email notifications are properly configured
    /// </summary>
    public bool IsEmailConfigured { get; init; }
}
