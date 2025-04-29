using InfoSymbolServer.Domain.Abstractions;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;
using InfoSymbolServer.Infrastructure.Abstractions;
using InfoSymbolServer.Infrastructure.Notifications.Formatters;
using InfoSymbolServer.Infrastructure.Notifications.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace InfoSymbolServer.Infrastructure.Notifications;

/// <summary>
/// Sends normal notifications via Email
/// </summary>
public class EmailNotificationService : INotificationService
{
    private readonly EmailNotificationOptions _emailOptions;
    private readonly SmtpSettings _smtpSettings;
    private readonly INotificationMessageFormatter _messageFormatter;
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly INotificationSettingsRepository _notificationSettingsRepository;
    private readonly INotificationValidationService _notificationValidationService;
    
    public EmailNotificationService(
        IOptions<EmailNotificationOptions> emailOptions,
        IOptions<SmtpSettings> smtpSettings,
        ILogger<EmailNotificationService> logger,
        INotificationSettingsRepository notificationSettingsRepository,
        INotificationValidationService notificationValidationService,
        IOptions<TelegramNotificationOptions> telegramOptions)
    {
        _emailOptions = emailOptions.Value;
        _smtpSettings = smtpSettings.Value;
        _messageFormatter = new HtmlNotificationMessageFormatter(telegramOptions);
        _logger = logger;
        _notificationSettingsRepository = notificationSettingsRepository;
        _notificationValidationService = notificationValidationService;
    }

    /// <inheritdoc />
    public async Task<bool> AreNotificationsEnabledAsync()
    {
        if (!_notificationValidationService.IsEmailValid())
        {
            return false;
        }
        
        var settings = await _notificationSettingsRepository.GetAsync();
        return settings?.IsEmailEnabled ?? true;
    }

    /// <inheritdoc />
    public async Task SendNewSymbolsNotificationAsync(IEnumerable<Symbol> symbols, string exchangeName)
    {
        // Check if Email notifications are enabled
        if (!await AreNotificationsEnabledAsync())
        {
            _logger.LogWarning("Attempted to send new symbols notification for {ExchangeName}, but Email notifications are disabled", 
                exchangeName);
            return;
        }
        
        var symbolsList = symbols.ToList();
        if (symbolsList.Count == 0)
        {
            _logger.LogWarning("No new symbols to send notification about for {ExchangeName}", exchangeName);
            return;
        }
        
        var message = _messageFormatter.FormatNewSymbolsMessage(symbolsList, exchangeName);
        await SendEmailNotificationAsync($"New Symbols on {exchangeName}", message);
    }

    /// <inheritdoc />
    public async Task SendUpdatedSymbolsNotificationAsync(IEnumerable<Symbol> symbols, string exchangeName)
    {
        // Check if Email notifications are enabled
        if (!await AreNotificationsEnabledAsync())
        {
            _logger.LogWarning("Attempted to send updated symbols notification for {ExchangeName}, but Email notifications are disabled", 
                exchangeName);
            return;
        }
        
        var symbolsList = symbols.ToList();
        if (symbolsList.Count == 0)
        {
            _logger.LogWarning("No updated symbols to send notification about for {ExchangeName}", exchangeName);
            return;
        }
        
        var message = _messageFormatter.FormatUpdatedSymbolsMessage(symbolsList, exchangeName);
        await SendEmailNotificationAsync($"Updated Symbols on {exchangeName}", message);
    }
    
    /// <inheritdoc />
    public async Task SendDelistedSymbolsNotificationAsync(IEnumerable<Symbol> symbols, string exchangeName)
    {
        // Check if Email notifications are enabled
        if (!await AreNotificationsEnabledAsync())
        {
            _logger.LogWarning("Attempted to send delisted symbols notification for {ExchangeName}, but Email notifications are disabled", 
                exchangeName);
            return;
        }
        
        var symbolsList = symbols.ToList();
        if (symbolsList.Count == 0)
        {
            _logger.LogWarning("No delisted symbols to send notification about for {ExchangeName}", exchangeName);
            return;
        }
        
        var message = _messageFormatter.FormatDelistedSymbolsMessage(symbolsList, exchangeName);
        await SendEmailNotificationAsync($"Delisted Symbols on {exchangeName}", message);
    }
    
    /// <inheritdoc />
    public async Task SendCombinedSymbolChangesNotificationAsync(
        IEnumerable<Symbol> newSymbols, 
        IEnumerable<Symbol> updatedSymbols, 
        IEnumerable<Symbol> delistedSymbols,
        string exchangeName, 
        string marketType)
    {
        // Check if Email notifications are enabled
        if (!await AreNotificationsEnabledAsync())
        {
            _logger.LogWarning("Attempted to send combined symbols notification for {ExchangeName} ({MarketType}), but Email notifications are disabled", 
                exchangeName, marketType);
            return;
        }
        
        var newSymbolsList = newSymbols.ToList();
        var updatedSymbolsList = updatedSymbols.ToList();
        var delistedSymbolsList = delistedSymbols.ToList();
        
        if (newSymbolsList.Count == 0 && updatedSymbolsList.Count == 0 && delistedSymbolsList.Count == 0)
        {
            _logger.LogWarning("No symbol changes to send notification about for {ExchangeName} {MarketType}", 
                exchangeName, marketType);
            return;
        }
        
        var message = _messageFormatter.FormatCombinedSymbolChangesMessage(
            newSymbolsList, updatedSymbolsList, delistedSymbolsList, exchangeName, marketType);
        
        await SendEmailNotificationAsync($"Symbol Changes on {exchangeName} ({marketType})", message);
    }

    private async Task SendEmailNotificationAsync(string subject, string body)
    {
        if (string.IsNullOrEmpty(body))
        {
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
            foreach (var recipient in _emailOptions.Recipients)
            {
                if (!string.IsNullOrWhiteSpace(recipient))
                {
                    mailMessage.To.Add(recipient);
                }
            }
            
            // Skip if no valid recipients
            if (mailMessage.To.Count == 0)
            {
                _logger.LogWarning("Email notification not sent: no valid recipients configured.");
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
                "Email notification sent successfully to {RecipientCount} recipients",
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