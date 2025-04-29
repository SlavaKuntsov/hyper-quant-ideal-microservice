namespace InfoSymbolServer.Domain.Abstractions;

/// <summary>
/// Service for validating notification channel configuration status
/// </summary>
public interface INotificationValidationService
{
    /// <summary>
    /// Checks if Telegram notifications are properly configured
    /// </summary>
    /// <returns>True if Telegram is configured, false otherwise</returns>
    bool IsTelegramValid();
    
    /// <summary>
    /// Checks if Email notifications are properly configured
    /// </summary>
    /// <returns>True if Email is configured, false otherwise</returns>
    bool IsEmailValid();

    /// <summary>
    /// Checks if Emergency Email notifications are properly configured
    /// </summary>
    /// <returns>True if Emergency Email is configured, false otherwise</returns>
    bool IsEmergencyEmailValid();
}
