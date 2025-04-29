using InfoSymbolServer.Domain.Abstractions;
using InfoSymbolServer.Domain.Repositories;
using InfoSymbolServer.Infrastructure.Abstractions;
using InfoSymbolServer.Infrastructure.BackgroundJobs;
using InfoSymbolServer.Infrastructure.DataAccess;
using InfoSymbolServer.Infrastructure.DataAccess.Repositories;
using InfoSymbolServer.Infrastructure.Notifications;
using InfoSymbolServer.Infrastructure.Notifications.Formatters;
using InfoSymbolServer.Infrastructure.Notifications.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace InfoSymbolServer.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure Infrastructure project.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers infrastructure services including database context and repositories.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The application configuration</param>
    /// <param name="environment">The hosting environment</param>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(AssemblyReference.Assembly))
                .UseSnakeCaseNamingConvention();

            // Includes more logging info for debugging purposes.
            if (environment.IsDevelopment())
            {
                options
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        });

        // Register repositories & unit of work implementations.
        services.AddScoped<IExchangeRepository, ExchangeRepository>();
        services.AddScoped<ISymbolRepository, SymbolRepository>();
        services.AddScoped<IStatusRepository, StatusRepository>();
        services.AddScoped<INotificationSettingsRepository, NotificationSettingsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.ConfigureQuarts(configuration);

        // Registers Binance rest and websocket clients to access exchange information.
        services.AddBinance();

        services.Configure<TelegramNotificationOptions>(
            configuration.GetSection("Notifications:Telegram"));
            
        // Configure SMTP settings
        services.Configure<SmtpSettings>(
            configuration.GetSection("Notifications:Smtp"));
            
        // Configure email notification recipients
        services.Configure<EmailNotificationOptions>(
            configuration.GetSection("Notifications:Email"));
            
        // Configure emergency email notification recipients
        services.Configure<EmergencyEmailOptions>(
            configuration.GetSection("Notifications:EmailEmergency"));

        // Register notification validation service
        services.AddScoped<INotificationValidationService, NotificationValidationService>();
        
        // Register notification services
        services.AddScoped<INotificationService, TelegramNotificationService>();
        services.AddScoped<INotificationService, EmailNotificationService>();
        services.AddScoped<IEmergencyNotificationService, EmailEmergencyNotificationService>();
        
        return services;
    }

    private static void ConfigureQuarts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(options =>
        {
            // Configures PostgreSQL as persistent jobs storage.
            options.UsePersistentStore(configure =>
            {
                configure.UsePostgres(configuration.GetConnectionString("DefaultConnection")!);
                configure.UseNewtonsoftJsonSerializer();
            });

            var schedule = configuration.GetValue<string>("BackgroundJobs:BinanceSymbolSyncJobSchedule")!;

            // Register Binance symbol sync jobs
            RegisterSymbolSyncJob<BinanceSpotSymbolSyncJob>(
                options, configuration, schedule);
            RegisterSymbolSyncJob<BinanceUsdtFuturesSymbolSyncJob>(
                options, configuration, schedule);
            RegisterSymbolSyncJob<BinanceCoinFuturesSymbolSyncJob>(
                options, configuration, schedule);
        });

        services.AddQuartzHostedService(options => 
        {
            options.WaitForJobsToComplete = true;
        });
    }

    /// <summary>
    /// Registers a symbol synchronization job with its trigger in Quartz.
    /// </summary>
    /// <typeparam name="TJob">Type of the job to register</typeparam>
    /// <param name="options">Quartz configuration options</param>
    /// <param name="configuration">Application configuration</param>
    /// <param name="schedule">Cron schedule</param>
    private static void RegisterSymbolSyncJob<TJob>(
        IServiceCollectionQuartzConfigurator options, 
        IConfiguration configuration, 
        string schedule) where TJob : IJob
    {
        var jobKey = JobKey.Create(typeof(TJob).Name);
        
        options
            .AddJob<TJob>(jobBuilder => jobBuilder
                .WithIdentity(jobKey)
                .DisallowConcurrentExecution()
                .RequestRecovery()
            )
            .AddTrigger(triggerBuilder => triggerBuilder
                .ForJob(jobKey)
                .WithIdentity($"{jobKey.Name}-Trigger")
                .WithCronSchedule(schedule, cronBuilder => 
                    cronBuilder.WithMisfireHandlingInstructionFireAndProceed()
                )
            );
    }
}
