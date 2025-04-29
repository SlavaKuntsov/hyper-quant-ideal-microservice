using AutoMapper;
using InfoSymbolServer.Application.Abstractions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Domain.Abstractions;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;

namespace InfoSymbolServer.Application.Services;

/// <summary>
/// Service for managing notification settings
/// </summary>
public class NotificationSettingsService(
    INotificationSettingsRepository repository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    INotificationValidationService validationService): INotificationSettingsService
{
    /// <inheritdoc />
    public async Task<NotificationSettingsDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var settings = await repository.GetAsync(cancellationToken);
        
        // If settings don't exist yet, create default settings
        if (settings == null)
        {
            settings = new NotificationSettings
            {
                IsTelegramEnabled = true,
                IsEmailEnabled = true
            };

            await repository.UpdateAsync(settings, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);
        }
        
        var isTelegramConfigured = validationService.IsTelegramValid();
        var isEmailConfigured = validationService.IsEmailValid();

        // Add configuration status information
        return new NotificationSettingsDto()
        {
            IsTelegramConfigured = isTelegramConfigured,
            IsEmailConfigured = isEmailConfigured,
            IsTelegramEnabled = settings.IsTelegramEnabled && isTelegramConfigured,
            IsEmailEnabled = settings.IsEmailEnabled && isEmailConfigured
        };
    }

    /// <inheritdoc />
    public async Task<NotificationSettingsDto> UpdateAsync(
        UpdateNotificationSettingsDto updateDto, CancellationToken cancellationToken = default)
    {
        // Validate if user can enable notifications
        ValidateNotificationSettings(updateDto);

        var settings = await repository.GetAsync(cancellationToken);
        
        // If settings don't exist yet, create default settings
        settings ??= new NotificationSettings
            {
                IsTelegramEnabled = true,
                IsEmailEnabled = true
            };
        
        settings.IsTelegramEnabled = updateDto.IsTelegramEnabled;
        settings.IsEmailEnabled = updateDto.IsEmailEnabled;

        await repository.UpdateAsync(settings, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        
        return mapper.Map<NotificationSettingsDto>(settings) with
        {
            IsTelegramConfigured = validationService.IsTelegramValid(),
            IsEmailConfigured = validationService.IsEmailValid()
        };
    }
    
    private void ValidateNotificationSettings(UpdateNotificationSettingsDto updateDto)
    {
        var errors = new Dictionary<string, string>();
        
        // If tries to enable Telegram notifications and they are not properly configured, add an error
        if (updateDto.IsTelegramEnabled && !validationService.IsTelegramValid())
        {
            errors.Add(
                nameof(updateDto.IsTelegramEnabled),
                "Cannot enable Telegram notifications because they are not properly configured");
        }
        
        // If tries to enable Email notifications and they are not properly configured, add an error
        if (updateDto.IsEmailEnabled && !validationService.IsEmailValid())
        {
            errors.Add(
                nameof(updateDto.IsEmailEnabled),
                "Cannot enable Email notifications because they are not properly configured");
        }
        
        if (errors.Count != 0)
        {
            throw new Exceptions.ValidationException(errors);
        }
    }
}
