using InfoSymbolServer.Domain.Abstractions;
using InfoSymbolServer.Infrastructure.Notifications.Options;
using Microsoft.Extensions.Options;

namespace InfoSymbolServer.Infrastructure.Notifications;

/// <summary>
/// Service for validating notification channel configuration status
/// </summary>
public class NotificationValidationService(
    IOptions<TelegramNotificationOptions> telegramOptions,
    IOptions<EmailNotificationOptions> emailOptions,
    IOptions<EmergencyEmailOptions> emergencyEmailOptions,
    IOptions<SmtpSettings> smtpSettings) : INotificationValidationService
{
    /// <inheritdoc />
    public bool IsTelegramValid()
    {
        return telegramOptions.Value.IsValid();
    }

    /// <inheritdoc />
    public bool IsEmailValid()
    {
        return smtpSettings.Value.IsValid() && emailOptions.Value.IsValid();
    }

    /// <inheritdoc />
    public bool IsEmergencyEmailValid()
    {
        return smtpSettings.Value.IsValid() && emergencyEmailOptions.Value.IsValid();
    }
} 