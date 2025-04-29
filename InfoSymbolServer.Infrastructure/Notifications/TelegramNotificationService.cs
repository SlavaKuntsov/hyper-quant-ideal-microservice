using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;
using InfoSymbolServer.Infrastructure.Abstractions;
using InfoSymbolServer.Infrastructure.Notifications.Formatters;
using InfoSymbolServer.Infrastructure.Notifications.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace InfoSymbolServer.Infrastructure.Notifications;

/// <summary>
/// Sends normal notifications via Telegram Bot 
/// </summary>
public class TelegramNotificationService : INotificationService
{
    private const int MaxMessageLength = 4096;

    private readonly TelegramNotificationOptions _options;
    private readonly INotificationMessageFormatter _messageFormatter;
    private readonly ILogger<TelegramNotificationService> _logger;
    private readonly ITelegramBotClient? _telegramBotClient;
    private readonly INotificationSettingsRepository _notificationSettingsRepository;
    
    public TelegramNotificationService(
        IOptions<TelegramNotificationOptions> options,
        ILogger<TelegramNotificationService> logger,
        INotificationSettingsRepository notificationSettingsRepository)
    {
        _options = options.Value;
        _messageFormatter = new MarkdownNotificationMessageFormatter(options);
        _logger = logger;
        _notificationSettingsRepository = notificationSettingsRepository;

        if (_options.IsValid())
        {
            try
            {
                _telegramBotClient = new TelegramBotClient(_options.BotToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to initialize Telegram bot client. Telegram notifications will be disabled. Error: {Error}",
                    ex.Message);
            }
        }
    }

    /// <inheritdoc />
    public async Task<bool> AreNotificationsEnabledAsync()
    {
        if (!_options.IsValid() || _telegramBotClient == null)
        {
            return false;
        }
        
        var settings = await _notificationSettingsRepository.GetAsync();
        return settings?.IsTelegramEnabled ?? true;
    }

    /// <inheritdoc />
    public async Task SendNewSymbolsNotificationAsync(IEnumerable<Symbol> symbols, string exchangeName)
    {
        // Check if Telegram notifications are enabled
        if (!await AreNotificationsEnabledAsync())
        {
            _logger.LogWarning("Attempted to send new symbols notification for {ExchangeName}, but Telegram notifications are disabled", 
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
        await SendTelegramMessageAsync(message);
    }

    /// <inheritdoc />
    public async Task SendUpdatedSymbolsNotificationAsync(IEnumerable<Symbol> symbols, string exchangeName)
    {
        // Check if Telegram notifications are enabled
        if (!await AreNotificationsEnabledAsync())
        {
            _logger.LogWarning("Attempted to send updated symbols notification for {ExchangeName}, but Telegram notifications are disabled", 
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
        await SendTelegramMessageAsync(message);
    }
    
    /// <inheritdoc />
    public async Task SendDelistedSymbolsNotificationAsync(IEnumerable<Symbol> symbols, string exchangeName)
    {
        // Check if Telegram notifications are enabled
        if (!await AreNotificationsEnabledAsync())
        {
            _logger.LogWarning("Attempted to send delisted symbols notification for {ExchangeName}, but Telegram notifications are disabled", 
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
        await SendTelegramMessageAsync(message);
    }
    
    /// <inheritdoc />
    public async Task SendCombinedSymbolChangesNotificationAsync(
        IEnumerable<Symbol> newSymbols, 
        IEnumerable<Symbol> updatedSymbols, 
        IEnumerable<Symbol> delistedSymbols, 
        string exchangeName,
        string marketType)
    {
        // Check if Telegram notifications are enabled
        if (!await AreNotificationsEnabledAsync())
        {
            _logger.LogWarning("Attempted to send combined symbols notification for {ExchangeName} ({MarketType}), but Telegram notifications are disabled", 
                exchangeName, marketType);
            return;
        }
        
        var newSymbolsList = newSymbols.ToList();
        var updatedSymbolsList = updatedSymbols.ToList();
        var delistedSymbolsList = delistedSymbols.ToList();
        
        // Only send notification if there are changes
        if (newSymbolsList.Count == 0 && updatedSymbolsList.Count == 0 && delistedSymbolsList.Count == 0)
        {
            _logger.LogInformation("No symbol changes to send notification about for {ExchangeName} ({MarketType})", 
                exchangeName, marketType);
            return;
        }
        
        _logger.LogInformation(
            "Sending combined notification for {ExchangeName} ({MarketType}): {NewCount} new, {UpdatedCount} updated, {DelistedCount} delisted", 
            exchangeName, marketType, newSymbolsList.Count, updatedSymbolsList.Count, delistedSymbolsList.Count);
        
        var message = _messageFormatter.FormatCombinedSymbolChangesMessage(
            newSymbolsList, updatedSymbolsList, delistedSymbolsList, exchangeName, marketType);
            
        await SendTelegramMessageAsync(message);
    }

    private async Task SendTelegramMessageAsync(string message)
    {
        if (string.IsNullOrEmpty(message) || _telegramBotClient == null)
        {
            return;
        }

        var sentCount = 0;
        var errorCount = 0;

        // Split message into chunks if it exceeds maximum length
        var messageChunks = SplitMessageIntoChunks(message);
        
        // Send message to each recipient
        foreach (var chatId in _options.ChatIds)
        {
            if (string.IsNullOrWhiteSpace(chatId))
            {
                continue;
            }
            
            try
            {
                // Send each chunk as a separate message
                foreach (var chunk in messageChunks)
                {
                    await _telegramBotClient.SendMessage(
                        chatId: chatId,
                        text: chunk,
                        parseMode: ParseMode.Markdown);
                }
                sentCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Telegram notification to chat ID {ChatId}: {Message}", 
                    chatId, ex.Message);
                errorCount++;
            }
        }

        if (sentCount > 0)
        {
            _logger.LogInformation(
                "Telegram notification sent successfully to {SuccessCount} recipients (failed: {FailedCount})",
                sentCount, errorCount);
        }
    }
    
    private List<string> SplitMessageIntoChunks(string message)
    {
        var chunks = new List<string>();
        
        // If message is already within limits, return it as is
        if (message.Length <= MaxMessageLength)
        {
            chunks.Add(message);
            return chunks;
        }
        
        _logger.LogInformation(
            "Message length ({Length}) exceeds maximum allowed size. Splitting into chunks.", message.Length);
        
        // Find appropriate break points (prefer line breaks, then spaces)
        var startIndex = 0;
        while (startIndex < message.Length)
        {
            var endIndex = startIndex + MaxMessageLength;
            if (endIndex > message.Length)
            {
                endIndex = message.Length;
            }
            else
            {
                // Try to find a line break to break at
                var lineBreakPos = message
                    .LastIndexOf('\n', endIndex - 1, Math.Min(MaxMessageLength, endIndex - startIndex));

                if (lineBreakPos > startIndex)
                {
                    endIndex = lineBreakPos + 1; // Include the line break
                }
                else
                {
                    // If no line break, try to find a space
                    var spacePos = message
                        .LastIndexOf(' ', endIndex - 1, Math.Min(MaxMessageLength, endIndex - startIndex));

                    if (spacePos > startIndex)
                    {
                        endIndex = spacePos + 1; // Include the space
                    }
                }
            }
            
            chunks.Add(message.Substring(startIndex, endIndex - startIndex));
            startIndex = endIndex;
        }
        
        _logger.LogInformation("Split message into {ChunkCount} chunks", chunks.Count);
        return chunks;
    }
}
