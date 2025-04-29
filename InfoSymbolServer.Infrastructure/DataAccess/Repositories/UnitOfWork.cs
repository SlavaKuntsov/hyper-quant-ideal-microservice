using InfoSymbolServer.Domain.Repositories;

namespace InfoSymbolServer.Infrastructure.DataAccess.Repositories;

/// <inheritdoc/>
public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    /// <inheritdoc/>
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
