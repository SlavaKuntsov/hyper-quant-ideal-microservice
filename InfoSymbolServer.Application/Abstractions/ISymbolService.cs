using System.Linq.Expressions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Domain.Models;

namespace InfoSymbolServer.Application.Abstractions;

/// <summary>
/// Service for managing symbols
/// </summary>
public interface ISymbolService
{
    /// <summary>
    /// Gets symbols for a specific exchange
    /// </summary>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of symbols for the specified exchange</returns>
    Task<IEnumerable<SymbolDto>> GetForExchangeAsync(
        string exchangeName, 
        int? pageNumber = null,
        int? pageSize = null, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets active symbols for a specific exchange
    /// </summary>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of active symbols for the specified exchange</returns>
    Task<IEnumerable<SymbolDto>> GetActiveForExchangeAsync(
        string exchangeName, 
        int? pageNumber = null,
        int? pageSize = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a symbol by its name and exchange name
    /// </summary>
    /// <param name="symbolName">The symbol name</param>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The symbol if found, otherwise null</returns>
    Task<SymbolDto?> GetForExchangeByNameAsync(
        string symbolName, string exchangeName, CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Adds a new symbol. Will fail if the symbol already exists.
    /// </summary>
    /// <param name="dto">The symbol data to add (including exchange name)</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The added symbol</returns>
    Task<SymbolDto> AddAsync(AddSymbolDto dto, CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Revokes the deleted status of a symbol by changing its status from RemovedByAdmin to AddedByAdmin
    /// </summary>
    /// <param name="symbolName">The symbol name</param>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated symbol</returns>
    Task<SymbolDto> RevokeDeleteAsync(
        string symbolName, string exchangeName, CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Marks a symbol as deleted by admin (sets status to RemovedByAdmin)
    /// </summary>
    /// <param name="symbolName">The symbol name</param>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated symbol</returns>
    Task<SymbolDto> DeleteAsync(
        string symbolName, string exchangeName, CancellationToken cancellationToken = default);
}
