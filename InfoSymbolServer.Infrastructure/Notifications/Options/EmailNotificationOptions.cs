namespace InfoSymbolServer.Infrastructure.Notifications.Options;

/// <summary>
/// Configuration options for email notification service
/// </summary>
public class EmailNotificationOptions
{
    /// <summary>
    /// Recipient email addresses
    /// </summary>
    public string[] Recipients { get; set; } = [];
    
    /// <summary>
    /// Validates if recipients are properly configured
    /// </summary>
    /// <returns>True if properly configured, otherwise false</returns>
    public bool IsValid()
    {
        return Recipients.Length > 0 &&
               !Recipients.All(string.IsNullOrWhiteSpace);
    }
}
