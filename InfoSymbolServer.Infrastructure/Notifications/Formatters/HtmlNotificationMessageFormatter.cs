using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Infrastructure.Abstractions;
using InfoSymbolServer.Infrastructure.Notifications.Options;
using Microsoft.Extensions.Options;
using System.Text;

namespace InfoSymbolServer.Infrastructure.Notifications.Formatters;

/// <summary>
/// Formats messages to HTML for symbol notifications in email
/// </summary>
public class HtmlNotificationMessageFormatter(IOptions<TelegramNotificationOptions> options)
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
        messageBuilder.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 800px;'>");
        messageBuilder.AppendLine($"<h2>📊 Symbol changes on {exchangeName} ({marketType})</h2>");
        messageBuilder.AppendLine($"<p><em>{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</em></p>");
        
        // Add section for new symbols if any
        AppendSymbolsSection(messageBuilder, newSymbolsList, "🆕", "New Symbols");
        
        // Add section for updated symbols if any
        AppendSymbolsSection(messageBuilder, updatedSymbolsList, "🔄", "Updated Symbols");
        
        // Add section for delisted symbols if any
        AppendSymbolsSection(messageBuilder, delistedSymbolsList, "⛔", "Delisted Symbols");
        
        // Add summary
        var totalChanges = newSymbolsList.Count + updatedSymbolsList.Count + delistedSymbolsList.Count;
        messageBuilder.AppendLine($"<p><strong>Total changes: {totalChanges}</strong></p>");
        messageBuilder.AppendLine("</div>");
        
        return messageBuilder.ToString();
    }

    /// <summary>
    /// Formats a simple message for new or updated symbols
    /// </summary>
    private string FormatSimpleSymbolsMessage(
        IEnumerable<Symbol> symbols, string exchangeName, string icon, string prefix)
    {
        var symbolsList = symbols.ToList();
        if (symbolsList.Count == 0)
        {
            return string.Empty;
        }
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 800px;'>");
        messageBuilder.AppendLine($"<h2>{icon} {prefix} {exchangeName}</h2>");
        
        FormatSymbolsList(symbolsList, messageBuilder);
        
        messageBuilder.AppendLine($"<p><strong>Total: {symbolsList.Count} symbol(s)</strong></p>");
        messageBuilder.AppendLine($"<p><em>Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</em></p>");
        messageBuilder.AppendLine("</div>");
        
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
            messageBuilder.AppendLine($"<h3>{icon} {title} ({symbols.Count})</h3>");
            FormatSymbolsList(symbols, messageBuilder);
        }
    }

    /// <summary>
    /// Formats the list of symbols into HTML
    /// </summary>
    private void FormatSymbolsList(List<Symbol> symbols, StringBuilder messageBuilder)
    {
        var symbolsToShow = symbols.Take(_options.MaxSymbolsPerMessage).ToList();
        var remainingCount = symbols.Count - symbolsToShow.Count;
        
        messageBuilder.AppendLine("<ul style='list-style-type: none; padding-left: 10px;'>");
        
        foreach (var symbol in symbolsToShow)
        {
            messageBuilder.AppendLine("<li style='margin-bottom: 10px;'>");
            messageBuilder.AppendLine($"<code style='background-color: #f5f5f5; padding: 2px 4px; border-radius: 3px;'>{symbol.SymbolName}</code> ({symbol.MarketType})");
            
            if (_options.IncludeDetailedInfo)
            {
                messageBuilder.AppendLine("<div style='margin-left: 20px;'>");
                messageBuilder.AppendLine($"Base: <code style='background-color: #f5f5f5; padding: 2px 4px; border-radius: 3px;'>{symbol.BaseAsset}</code>, ");
                messageBuilder.AppendLine($"Quote: <code style='background-color: #f5f5f5; padding: 2px 4px; border-radius: 3px;'>{symbol.QuoteAsset}</code><br>");
                messageBuilder.AppendLine($"Status: {symbol.Status}");
                messageBuilder.AppendLine("</div>");
            }
            
            messageBuilder.AppendLine("</li>");
        }
        
        messageBuilder.AppendLine("</ul>");
        
        if (remainingCount > 0)
        {
            messageBuilder.AppendLine($"<p><em>...and {remainingCount} more symbols</em></p>");
        }
    }
}
