using AutoMapper;
using InfoSymbolServer.Application.Abstractions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Presentation.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoSymbolServer.Presentation.Controllers;

/// <summary>
/// API Controller for managing notification settings
/// </summary>
[ApiController]
[Route("api/v1/notification-settings")]
[Produces("application/json")]
[Tags("Notification Settings")]
public class NotificationSettingsController : ControllerBase
{
    private readonly INotificationSettingsService _notificationSettingsService;
    private readonly IMapper _mapper;

    public NotificationSettingsController(INotificationSettingsService notificationSettingsService, IMapper mapper)
    {
        _notificationSettingsService = notificationSettingsService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets the current notification settings
    /// </summary>
    /// <returns>The current notification settings</returns>
    /// <response code="200">Returns the notification settings</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationSettingsDto>> GetSettings()
    {
        var settings = await _notificationSettingsService.GetAsync();
        return Ok(settings);
    }

    /// <summary>
    /// Updates the notification settings
    /// </summary>
    /// <param name="request">The updated settings</param>
    /// <returns>The updated notification settings</returns>
    /// <response code="200">Returns the updated notification settings</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationSettingsDto>> UpdateSettings(
        [FromBody] UpdateNotificationSettingsRequest request)
    {
        var updateDto = _mapper.Map<UpdateNotificationSettingsDto>(request);
        var updatedSettings = await _notificationSettingsService.UpdateAsync(updateDto);
        return Ok(updatedSettings);
    }
}
