using InfoSymbolServer.Application.Dtos;

namespace InfoSymbolServer.Application.Abstractions;

/// <summary>
/// Service for managing notification settings
/// </summary>
public interface INotificationSettingsService
{
    /// <summary>
    /// Gets the current notification settings
    /// </summary>
    /// <returns>The notification settings DTO</returns>
    Task<NotificationSettingsDto> GetAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates the notification settings
    /// </summary>
    /// <param name="updateDto">The DTO containing the updated settings</param>
    /// <returns>The updated notification settings DTO</returns>
    Task<NotificationSettingsDto> UpdateAsync(UpdateNotificationSettingsDto updateDto, CancellationToken cancellationToken = default);
} 