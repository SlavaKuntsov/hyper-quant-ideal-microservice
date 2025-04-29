using System.Linq.Expressions;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InfoSymbolServer.Infrastructure.DataAccess.Repositories;

/// <inheritdoc/>
public class ExchangeRepository(ApplicationDbContext context) : IExchangeRepository
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Exchange>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Exchanges
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Exchange>> GetByFilterAsync(
        Expression<Func<Exchange, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await context.Exchanges
            .Where(filter)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<Exchange?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.Exchanges
            .SingleOrDefaultAsync(e => e.Name == name, cancellationToken);
    }

    /// <inheritdoc/>
    public Task AddAsync(Exchange exchange, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(exchange);

        context.Exchanges.Add(exchange);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Exchange exchange, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(exchange);

        context.Exchanges.Update(exchange);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task DeleteAsync(Exchange exchange, CancellationToken cancellationToken = default)
    {
        context.Exchanges.Remove(exchange);

        return Task.CompletedTask;
    }
}