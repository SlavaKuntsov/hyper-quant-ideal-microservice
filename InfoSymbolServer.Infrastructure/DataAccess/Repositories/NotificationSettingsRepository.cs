using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;
using InfoSymbolServer.Infrastructure.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace InfoSymbolServer.Infrastructure.DataAccess.Repositories;

/// <summary>
/// Repository for managing notification settings data
/// </summary>
public class NotificationSettingsRepository(ApplicationDbContext dbContext)
    : INotificationSettingsRepository
{
    /// <inheritdoc />
    public async Task<NotificationSettings?> GetAsync(CancellationToken cancellationToken = default)
    {
        // Always return the first record, as we only have one settings record in the table
        return await dbContext.NotificationSettings.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(NotificationSettings settings, CancellationToken cancellationToken = default)
    {
        dbContext.NotificationSettings.Update(settings);
        return Task.CompletedTask;
    }
}
