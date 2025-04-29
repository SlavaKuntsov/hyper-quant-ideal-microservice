using InfoSymbolServer.Domain.Models;

namespace InfoSymbolServer.Infrastructure.Abstractions;

/// <summary>
/// Service for sending normal notifications related to symbols updates
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Checks if notifications are currently enabled
    /// </summary>
    /// <returns>True if notifications are enabled, false otherwise</returns>
    Task<bool> AreNotificationsEnabledAsync();
    
    /// <summary>
    /// Sends notification about new symbols added to the system
    /// </summary>
    /// <param name="symbols">The list of new symbols</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendNewSymbolsNotificationAsync(IEnumerable<Symbol> symbols, string exchangeName);
    
    /// <summary>
    /// Sends notification about symbols that were updated in the system
    /// </summary>
    /// <param name="symbols">The list of updated symbols</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendUpdatedSymbolsNotificationAsync(IEnumerable<Symbol> symbols, string exchangeName);
    
    /// <summary>
    /// Sends notification about symbols that were delisted or removed from the system
    /// </summary>
    /// <param name="symbols">The list of delisted symbols</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendDelistedSymbolsNotificationAsync(IEnumerable<Symbol> symbols, string exchangeName);
    
    /// <summary>
    /// Sends a combined notification for all symbol changes (new, updated, and delisted)
    /// </summary>
    /// <param name="newSymbols">The list of new symbols</param>
    /// <param name="updatedSymbols">The list of updated symbols</param>
    /// <param name="delistedSymbols">The list of delisted symbols</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="marketType">The market type</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendCombinedSymbolChangesNotificationAsync(
        IEnumerable<Symbol> newSymbols, 
        IEnumerable<Symbol> updatedSymbols, 
        IEnumerable<Symbol> delistedSymbols, 
        string exchangeName,
        string marketType);
}
