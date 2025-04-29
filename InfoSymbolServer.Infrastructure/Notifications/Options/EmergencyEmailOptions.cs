namespace InfoSymbolServer.Infrastructure.Notifications.Options;

/// <summary>
/// Configuration options for emergency email notification service
/// </summary>
public class EmergencyEmailOptions
{
    /// <summary>
    /// Recipient email addresses for emergency notifications
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