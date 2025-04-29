using System.Linq.Expressions;
using InfoSymbolServer.Domain.Models;

namespace InfoSymbolServer.Domain.Repositories;

/// <summary>
/// Repository to operate with symbols
/// </summary>
public interface ISymbolRepository
{
    /// <summary>
    /// Gets symbols that match the specified filter
    /// </summary>
    /// <param name="filter">Expression to filter symbols</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of filtered symbols</returns>
    Task<IEnumerable<Symbol>> GetByFilterAsync(
        Expression<Func<Symbol, bool>> filter, 
        int? pageNumber = null, 
        int? pageSize = null, 
        CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Gets a single symbol that matches the specified filter
    /// </summary>
    /// <param name="filter">Expression to filter symbols</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The symbol if found, otherwise null</returns>
    Task<Symbol?> GetSingleByFilterAsync(
        Expression<Func<Symbol, bool>> filter, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new symbol
    /// </summary>
    /// <param name="symbol">The symbol to add</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The added symbol with generated ID</returns>
    Task AddAsync(Symbol symbol, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a range of new symbols
    /// </summary>
    /// <param name="symbols">Symbols to add</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The added symbol with generated ID</returns>
    Task AddRangeAsync(IEnumerable<Symbol> symbols, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing symbol
    /// </summary>
    /// <param name="symbol">The symbol with updated values</param>
    /// <param name="cancellationToken"></param>
    Task UpdateAsync(Symbol symbol, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing symbol
    /// </summary>
    /// <param name="symbols">Symbols with updated values</param>
    /// <param name="cancellationToken"></param>
    Task UpdateRangeAsync(IEnumerable<Symbol> symbols, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a symbol
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="cancellationToken"></param>
    Task DeleteAsync(Symbol symbol, CancellationToken cancellationToken = default);
}
