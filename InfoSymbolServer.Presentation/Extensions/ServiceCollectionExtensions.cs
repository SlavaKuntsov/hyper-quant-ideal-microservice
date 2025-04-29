using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;
using InfoSymbolServer.Presentation.Filters;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace InfoSymbolServer.Presentation.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure Presentation project.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers presentation services including controllers, CORS, etc.
    /// </summary>
    /// <param name="services">The service collection</param>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services
            .AddControllers(options => options.Filters.Add<ValidatePaginationAttribute>())
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddEndpointsApiExplorer();

        // Configures Swagger, that will be used in Development environment.
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "InfoSymbolServer API",
                Version = "v1",
                Description = "API for managing exchange symbols and trading information"
            });

            // Configure xml documentation for Swagger endpoints and objects. 
            var presentationXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var presentationXmlPath = Path.Combine(AppContext.BaseDirectory, presentationXmlFile);
            if (File.Exists(presentationXmlPath))
            {
                options.IncludeXmlComments(presentationXmlPath);
            }
            
            var applicationXmlFile = "InfoSymbolServer.Application.xml";
            var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);
            if (File.Exists(applicationXmlPath))
            {
                options.IncludeXmlComments(applicationXmlPath);
            }
            
            // Configures enums to be serialized into strings instead of integers.
            options.UseInlineDefinitionsForEnums();;
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }
}
