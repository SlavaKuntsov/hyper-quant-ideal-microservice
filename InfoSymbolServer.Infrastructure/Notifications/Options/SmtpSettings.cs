namespace InfoSymbolServer.Infrastructure.Notifications.Options;

/// <summary>
/// Shared SMTP server configuration settings
/// </summary>
public class SmtpSettings
{
    /// <summary>
    /// SMTP server address
    /// </summary>
    public string SmtpServer { get; set; } = null!;
    
    /// <summary>
    /// SMTP server port
    /// </summary>
    public int SmtpPort { get; set; }
    
    /// <summary>
    /// Whether to use SSL for SMTP connection
    /// </summary>
    public bool UseSsl { get; set; }
    
    /// <summary>
    /// Email sender address
    /// </summary>
    public string SenderEmail { get; set; } = null!;
    
    /// <summary>
    /// Email sender display name
    /// </summary>
    public string SenderName { get; set; } = null!;
    
    /// <summary>
    /// Email account username
    /// </summary>
    public string Username { get; set; } = null!;
    
    /// <summary>
    /// Email account password
    /// </summary>
    public string Password { get; set; } = null!;
    
    /// <summary>
    /// Validates if the SMTP settings are properly configured
    /// </summary>
    /// <returns>True if properly configured, otherwise false</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(SmtpServer) &&
               SmtpPort > 0 &&
               !string.IsNullOrWhiteSpace(SenderEmail) &&
               !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password);
    }
}
