using InfoSymbolServer.Domain.Models;

namespace InfoSymbolServer.Infrastructure.Abstractions;

/// <summary>
/// Interface for creating formatter notification messages
/// </summary>
public interface INotificationMessageFormatter
{
    /// <summary>
    /// Formats a message for new symbols notification
    /// </summary>
    /// <param name="symbols">The list of new symbols</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <returns>The formatted message</returns>
    string FormatNewSymbolsMessage(IEnumerable<Symbol> symbols, string exchangeName);
    
    /// <summary>
    /// Formats a message for updated symbols notification
    /// </summary>
    /// <param name="symbols">The list of updated symbols</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <returns>The formatted message</returns>
    string FormatUpdatedSymbolsMessage(IEnumerable<Symbol> symbols, string exchangeName);
    
    /// <summary>
    /// Formats a message for delisted symbols notification
    /// </summary>
    /// <param name="symbols">The list of delisted symbols</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <returns>The formatted message</returns>
    string FormatDelistedSymbolsMessage(IEnumerable<Symbol> symbols, string exchangeName);
    
    /// <summary>
    /// Formats a combined message for all symbol changes (new, updated, and delisted)
    /// </summary>
    /// <param name="newSymbols">The list of new symbols</param>
    /// <param name="updatedSymbols">The list of updated symbols</param>
    /// <param name="delistedSymbols">The list of delisted symbols</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="marketType">The market type</param>
    /// <returns>The formatted message</returns>
    string FormatCombinedSymbolChangesMessage(
        IEnumerable<Symbol> newSymbols, 
        IEnumerable<Symbol> updatedSymbols, 
        IEnumerable<Symbol> delistedSymbols, 
        string exchangeName,
        string marketType);
}
