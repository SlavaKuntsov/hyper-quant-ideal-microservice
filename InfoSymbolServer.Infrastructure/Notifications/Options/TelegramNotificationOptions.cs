namespace InfoSymbolServer.Infrastructure.Notifications.Options;

/// <summary>
/// Configuration options for Telegram notification service
/// </summary>
public class TelegramNotificationOptions
{
    /// <summary>
    /// Telegram Bot API token
    /// </summary>
    public string BotToken { get; set; } = null!;
    
    /// <summary>
    /// Telegram chat IDs to send notifications to
    /// </summary>
    public string[] ChatIds { get; set; } = [];
    
    /// <summary>
    /// Whether to include detailed information in notifications
    /// </summary>
    public bool IncludeDetailedInfo { get; set; }
    
    /// <summary>
    /// Maximum number of symbols to include in a single message
    /// </summary>
    public int MaxSymbolsPerMessage { get; set; }
    
    /// <summary>
    /// Validates if the telegram options are properly configured
    /// </summary>
    /// <returns>True if properly configured, otherwise false</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(BotToken) &&
               ChatIds.Length > 0 &&
               !ChatIds.All(string.IsNullOrWhiteSpace);
    }
}
