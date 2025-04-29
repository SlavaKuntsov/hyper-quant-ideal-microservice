namespace InfoSymbolServer.Domain.Models;

/// <summary>
/// Settings for controlling notification behavior in the system
/// </summary>
public class NotificationSettings
{
    /// <summary>
    /// Gets or sets the ID of the notification settings
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets whether Telegram notifications are enabled
    /// </summary>
    public bool IsTelegramEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets whether Email notifications are enabled
    /// </summary>
    public bool IsEmailEnabled { get; set; }
}
