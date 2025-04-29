using InfoSymbolServer.Application.Dtos;

namespace InfoSymbolServer.Application.Abstractions;

/// <summary>
/// Service for managing symbol status records
/// </summary>
public interface IStatusService
{
    /// <summary>
    /// Gets status history for a specific symbol
    /// </summary>
    /// <param name="symbolName">The name of the symbol</param>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Symbol history response with status records for the specified symbol</returns>
    Task<SymbolHistoryDto> GetSymbolHistoryAsync(
        string symbolName, 
        string exchangeName, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets status history for all symbols in the specified exchange
    /// </summary>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of symbol history responses for all symbols in the exchange</returns>
    Task<IEnumerable<SymbolHistoryDto>> GetExchangeSymbolsHistoryAsync(
        string exchangeName, 
        int? pageNumber = null,
        int? pageSize = null, 
        CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Gets status history for active symbols in the specified exchange
    /// </summary>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of symbol history responses for active symbols in the exchange</returns>
    Task<IEnumerable<SymbolHistoryDto>> GetExchangeActiveSymbolsHistoryAsync(
        string exchangeName, 
        int? pageNumber = null,
        int? pageSize = null, 
        CancellationToken cancellationToken = default);
} 