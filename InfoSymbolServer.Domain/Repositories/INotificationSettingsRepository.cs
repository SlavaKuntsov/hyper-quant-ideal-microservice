using InfoSymbolServer.Domain.Models;

namespace InfoSymbolServer.Domain.Repositories;

/// <summary>
/// Repository for managing notification settings data
/// </summary>
public interface INotificationSettingsRepository
{
    /// <summary>
    /// Gets the current notification settings
    /// </summary>
    /// <returns>The notification settings entity</returns>
    Task<NotificationSettings?> GetAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates the notification settings
    /// </summary>
    /// <param name="settings">The settings to update</param>
    /// <returns>The updated notification settings</returns>
    Task UpdateAsync(NotificationSettings settings, CancellationToken cancellationToken = default);
}
