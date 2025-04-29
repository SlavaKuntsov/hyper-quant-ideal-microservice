using System.Linq.Expressions;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InfoSymbolServer.Infrastructure.DataAccess.Repositories;

/// <inheritdoc/>
public class StatusRepository(ApplicationDbContext context) : IStatusRepository
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Status>> GetAllAsync(
        int? pageNumber = null, 
        int? pageSize = null, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<Status> query = context.Statuses
            .Include(s => s.Symbol)
            .OrderByDescending(s => s.CreatedAt);
            
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
        
        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Status>> GetByFilterAsync(
        Expression<Func<Status, bool>> filter, 
        int? pageNumber = null, 
        int? pageSize = null, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<Status> query = context.Statuses
            .Include(s => s.Symbol)
            .Where(filter)
            .OrderByDescending(s => s.CreatedAt);
            
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
        
        return await query.ToListAsync(cancellationToken);
    }
    
    /// <inheritdoc/>
    public async Task<Status?> GetSingleByFilterAsync(
        Expression<Func<Status, bool>> filter, 
        CancellationToken cancellationToken = default)
    {
        return await context.Statuses
            .Include(s => s.Symbol)
            .SingleOrDefaultAsync(filter, cancellationToken);
    }

    /// <inheritdoc/>
    public Task AddAsync(Status status, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(status);

        context.Statuses.Add(status);
        
        return Task.CompletedTask;
    }
    
    /// <inheritdoc/>
    public Task AddRangeAsync(IEnumerable<Status> statuses, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(statuses);
        
        context.Statuses.AddRange(statuses);
        
        return Task.CompletedTask;
    }
} 