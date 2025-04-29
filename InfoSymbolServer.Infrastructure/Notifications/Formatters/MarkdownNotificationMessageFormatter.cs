using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Infrastructure.Notifications.Options;
using Microsoft.Extensions.Options;
using System.Text;
using InfoSymbolServer.Infrastructure.Abstractions;

namespace InfoSymbolServer.Infrastructure.Notifications.Formatters;

/// <summary>
/// Formats messages to markdown for symbol notifications
/// </summary>
public class MarkdownNotificationMessageFormatter(IOptions<TelegramNotificationOptions> options)
    : INotificationMessageFormatter
{
    private readonly TelegramNotificationOptions _options = options.Value;

    /// <inheritdoc />
    public string FormatNewSymbolsMessage(IEnumerable<Symbol> symbols, string exchangeName)
    {
        return FormatSimpleSymbolsMessage(symbols, exchangeName, "🆕", "New symbols added on");
    }

    /// <inheritdoc />
    public string FormatUpdatedSymbolsMessage(IEnumerable<Symbol> symbols, string exchangeName)
    {
        return FormatSimpleSymbolsMessage(symbols, exchangeName, "🔄", "Symbols updated on");
    }
    
    /// <inheritdoc />
    public string FormatDelistedSymbolsMessage(IEnumerable<Symbol> symbols, string exchangeName)
    {
        return FormatSimpleSymbolsMessage(symbols, exchangeName, "⛔", "Symbols delisted from");
    }
    
    /// <inheritdoc />
    public string FormatCombinedSymbolChangesMessage(
        IEnumerable<Symbol> newSymbols,
        IEnumerable<Symbol> updatedSymbols,
        IEnumerable<Symbol> delistedSymbols,
        string exchangeName,
        string marketType)
    {
        var newSymbolsList = newSymbols.ToList();
        var updatedSymbolsList = updatedSymbols.ToList();
        var delistedSymbolsList = delistedSymbols.ToList();
        
        // If there are no changes, return empty message
        if (newSymbolsList.Count == 0 && updatedSymbolsList.Count == 0 && delistedSymbolsList.Count == 0)
        {
            return string.Empty;
        }
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine($"📊 *Symbol changes on {exchangeName} ({marketType})*");
        messageBuilder.AppendLine($"_{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC_");
        messageBuilder.AppendLine();
        
        // Add section for new symbols if any
        AppendSymbolsSection(messageBuilder, newSymbolsList, "🆕", "New Symbols");
        
        // Add section for updated symbols if any
        AppendSymbolsSection(messageBuilder, updatedSymbolsList, "🔄", "Updated Symbols");
        
        // Add section for delisted symbols if any
        AppendSymbolsSection(messageBuilder, delistedSymbolsList, "⛔", "Delisted Symbols");
        
        // Add summary
        var totalChanges = newSymbolsList.Count + updatedSymbolsList.Count + delistedSymbolsList.Count;
        messageBuilder.AppendLine($"*Total changes: {totalChanges}*");
        
        return messageBuilder.ToString();
    }

    /// <summary>
    /// Formats a simple message for new or updated symbols
    /// </summary>
    /// <param name="symbols">The list of symbols</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="icon">The icon to use for the message</param>
    /// <param name="prefix">The prefix for the message</param>
    /// <returns>The formatted message</returns>
    private string FormatSimpleSymbolsMessage(
        IEnumerable<Symbol> symbols, string exchangeName, string icon, string prefix)
    {
        var symbolsList = symbols.ToList();
        if (symbolsList.Count == 0)
        {
            return string.Empty;
        }
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine($"{icon} *{prefix} {exchangeName}*");
        messageBuilder.AppendLine();
        
        FormatSymbolsList(symbolsList, messageBuilder);
        
        messageBuilder.AppendLine();
        messageBuilder.AppendLine($"Total: {symbolsList.Count} symbol(s)");
        messageBuilder.AppendLine($"Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        
        return messageBuilder.ToString();
    }

    /// <summary>
    /// Appends a section for new or updated symbols to the message
    /// </summary>
    private void AppendSymbolsSection(
        StringBuilder messageBuilder, List<Symbol> symbols, string icon, string title)
    {
        if (symbols.Count > 0)
        {
            messageBuilder.AppendLine($"{icon} *{title} ({symbols.Count})*");
            FormatSymbolsList(symbols, messageBuilder);
            messageBuilder.AppendLine();
        }
    }

    /// <summary>
    /// Formats the list of symbols to a string
    /// </summary>
    private void FormatSymbolsList(List<Symbol> symbols, StringBuilder messageBuilder)
    {
        var symbolsToShow = symbols.Take(_options.MaxSymbolsPerMessage).ToList();
        var remainingCount = symbols.Count - symbolsToShow.Count;
        
        foreach (var symbol in symbolsToShow)
        {
            messageBuilder.AppendLine($"- `{symbol.SymbolName}` ({symbol.MarketType})");
            
            if (_options.IncludeDetailedInfo)
            {
                messageBuilder.AppendLine($"  Base: `{symbol.BaseAsset}`, Quote: `{symbol.QuoteAsset}`");
                messageBuilder.AppendLine($"  Status: {symbol.Status}");
                messageBuilder.AppendLine();
            }
        }
        
        if (remainingCount > 0)
        {
            messageBuilder.AppendLine();
            messageBuilder.AppendLine($"_...and {remainingCount} more symbols_");
        }
    }
}
