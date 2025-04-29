using InfoSymbolServer.Domain.Abstractions;
using InfoSymbolServer.Infrastructure.Abstractions;
using InfoSymbolServer.Infrastructure.Notifications.Formatters;
using InfoSymbolServer.Infrastructure.Notifications.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace InfoSymbolServer.Infrastructure.Notifications;

/// <summary>
/// Sends emergency notifications via email
/// </summary>
public class EmailEmergencyNotificationService : IEmergencyNotificationService
{
    private readonly EmergencyEmailOptions _emergencyOptions;
    private readonly SmtpSettings _smtpSettings;
    private readonly IEmergencyNotificationMessageFormatter _messageFormatter;
    private readonly ILogger<EmailEmergencyNotificationService> _logger;
    private readonly INotificationValidationService _notificationValidationService;
    public EmailEmergencyNotificationService(
        IOptions<EmergencyEmailOptions> emergencyOptions,
        IOptions<SmtpSettings> smtpSettings,
        ILogger<EmailEmergencyNotificationService> logger,
        INotificationValidationService notificationValidationService)
    {
        _emergencyOptions = emergencyOptions.Value;
        _smtpSettings = smtpSettings.Value;
        _messageFormatter = new HtmlEmergencyNotificationMessageFormatter();
        _logger = logger;
        _notificationValidationService = notificationValidationService;
    }

    /// <inheritdoc />
    public async Task SendExchangeApiErrorNotificationAsync(
        string exchangeName, string errorMessage, Exception? exception = null)
    {
        var subject = $"ALERT: Exchange API Error - {exchangeName}";
        var body = _messageFormatter.FormatExchangeApiErrorMessage(
            exchangeName, errorMessage, exception);
            
        await SendEmailAsync(subject, body);
    }

    /// <inheritdoc />
    public async Task SendDatabaseErrorNotificationAsync(
        string operation, string errorMessage, Exception? exception = null)
    {
        var subject = $"ALERT: Database Error - {operation}";
        var body = _messageFormatter.FormatDatabaseErrorMessage(
            operation, errorMessage, exception);
            
        await SendEmailAsync(subject, body);
    }

    /// <inheritdoc />
    public async Task SendSystemErrorNotificationAsync(
        string component, string errorMessage, Exception? exception = null)
    {
        var subject = $"ALERT: System Error - {component}";
        var body = _messageFormatter.FormatSystemErrorMessage(
            component, errorMessage, exception);
            
        await SendEmailAsync(subject, body);
    }

    private async Task SendEmailAsync(string subject, string body)
    {
        if (!_notificationValidationService.IsEmergencyEmailValid())
        {
            _logger.LogWarning("Email emergency notification not sent: service is not properly configured.");
            return;
        }
        
        try
        {
            // Create message
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            // Add all recipients
            foreach (var recipient in _emergencyOptions.Recipients)
            {
                if (!string.IsNullOrWhiteSpace(recipient))
                {
                    mailMessage.To.Add(recipient);
                }
            }
            
            // Skip if no valid recipients
            if (mailMessage.To.Count == 0)
            {
                _logger.LogWarning("Email emergency notification not sent: no valid recipients configured.");
                return;
            }

            // Configure SMTP client
            using var smtpClient = new SmtpClient(_smtpSettings.SmtpServer, _smtpSettings.SmtpPort)
            {
                EnableSsl = _smtpSettings.UseSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password)
            };

            // Send email
            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation(
                "Emergency email notification sent successfully to {RecipientCount} recipients",
                mailMessage.To.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error sending email notification: {Message}. SMTP server: {Server}, Port: {Port}, SSL: {UseSsl}", 
                ex.Message, _smtpSettings.SmtpServer, _smtpSettings.SmtpPort, _smtpSettings.UseSsl);
        }
    }
}
