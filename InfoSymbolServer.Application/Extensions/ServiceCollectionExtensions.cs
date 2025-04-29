using System.Reflection;
using FluentValidation;
using InfoSymbolServer.Application.Abstractions;
using InfoSymbolServer.Application.Services;
using InfoSymbolServer.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace InfoSymbolServer.Application.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure Application project.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers application services including services, mapping and validation.
    /// </summary>
    /// <param name="services">The service collection</param>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISymbolService, SymbolService>();
        services.AddScoped<IExchangeService, ExchangeService>();
        services.AddScoped<IStatusService, StatusService>();
        services.AddScoped<INotificationSettingsService, NotificationSettingsService>();

        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddScoped<ValidationPipeline>();

        services.AddProblemDetails();
        
        return services;
    }
}
