using InfoSymbolServer.Domain.Abstractions;
using InfoSymbolServer.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfoSymbolServer.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IApplicationBuilder"/> to configure Infrastructure project.
/// </summary>
public static class ApplicationExtensions
{
    /// <summary>
    /// Applies pending database migrations to the database.
    /// </summary>
    /// <param name="app">The application builder</param>
    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations");
            throw;
        }

        return app;
    }
    
    /// <summary>
    /// Validates notification configurations and logs warnings for missing or invalid configurations
    /// </summary>
    /// <param name="app">The application builder</param>
    public static IApplicationBuilder ValidateNotificationConfigurations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
        
        try
        {
            var validationService = services.GetRequiredService<INotificationValidationService>();
            
            // Validate Telegram notifications
            if (!validationService.IsTelegramValid())
            {
                logger.LogWarning(
                    "Telegram notification service is not properly configured. " +
                    "Please provide a valid bot token and at least one chat ID in the configuration. " +
                    "Telegram notifications will be disabled until properly configured.");
            }
            
            // Validate Email notifications
            if (!validationService.IsEmailValid())
            {
                logger.LogWarning(
                    "Email notification service is not properly configured. " +
                    "Please provide valid SMTP server settings and at least one recipient in the configuration. " +
                    "Email notifications will be disabled until properly configured.");
            }

            // Validate Emergency Email notifications
            if (!validationService.IsEmergencyEmailValid())
            {
                logger.LogWarning(
                    "Emergency email notification service is not properly configured. " +
                    "Please provide valid SMTP server settings and at least one recipient in the configuration. " +
                    "Emergency email notifications will be disabled until properly configured.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while validating notification configurations");
        }
        
        return app;
    }
}
