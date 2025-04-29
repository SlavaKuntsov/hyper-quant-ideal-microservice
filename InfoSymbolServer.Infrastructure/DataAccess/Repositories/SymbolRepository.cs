using System.Linq.Expressions;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InfoSymbolServer.Infrastructure.DataAccess.Repositories;

/// <inheritdoc/>
public class SymbolRepository(ApplicationDbContext context) : ISymbolRepository
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Symbol>> GetByFilterAsync(
        Expression<Func<Symbol, bool>> filter, 
        int? pageNumber = null, 
        int? pageSize = null, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<Symbol> query = context.Symbols
            .Where(filter)
            .OrderBy(s => s.SymbolName);
            
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
        
        return await query.ToListAsync(cancellationToken);
    }
    
    /// <inheritdoc/>
    public async Task<Symbol?> GetSingleByFilterAsync(
        Expression<Func<Symbol, bool>> filter, 
        CancellationToken cancellationToken = default)
    {
        return await context.Symbols
            .SingleOrDefaultAsync(filter, cancellationToken);
    }

    /// <inheritdoc/>
    public Task AddAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        context.Symbols.Add(symbol);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task AddRangeAsync(IEnumerable<Symbol> symbols, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(symbols);
        
        context.Symbols.AddRange(symbols);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(symbol);
        
        context.Symbols.Update(symbol);
        
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<Symbol> symbols, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(symbols);
        
        context.Symbols.UpdateRange(symbols);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task DeleteAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        context.Symbols.Remove(symbol);

        return Task.CompletedTask;
    }
}
