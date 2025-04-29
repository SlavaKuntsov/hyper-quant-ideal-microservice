using InfoSymbolServer.Domain.Constants;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfoSymbolServer.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for seeding exchanges into the database
/// </summary>
public static class ExchangeSeedExtensions
{
    /// <summary>
    /// Seeds the database with supported exchanges if they do not already exist
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public async static Task SeedExchangesAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<IApplicationBuilder>>();
        var exchangeRepository = services.GetRequiredService<IExchangeRepository>();
        var unitOfWork = services.GetRequiredService<IUnitOfWork>();

        try
        {
            var supportedExchangeNames = new[]
            {
                SupportedExchanges.BinanceSpot,
                SupportedExchanges.BinanceUsdtFutures,
                SupportedExchanges.BinanceCoinFutures
            };

            await SeedExchangesAsync(exchangeRepository, unitOfWork, supportedExchangeNames, logger);
            logger.LogInformation("Exchanges seeded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding exchanges");
        }
    }

    private static async Task SeedExchangesAsync(
        IExchangeRepository exchangeRepository,
        IUnitOfWork unitOfWork,
        IEnumerable<string> exchangeNames,
        ILogger logger)
    {
        foreach (var exchangeName in exchangeNames)
        {
            var existingExchange = await exchangeRepository.GetByNameAsync(exchangeName);
            if (existingExchange == null)
            {
                var exchange = new Exchange
                {
                    Id = Guid.NewGuid(),
                    Name = exchangeName,
                    CreatedAt = DateTime.UtcNow
                };

                await exchangeRepository.AddAsync(exchange);
                logger.LogInformation("Added exchange: {ExchangeName}", exchangeName);
            }
            else
            {
                logger.LogInformation("Exchange already exists: {ExchangeName}", exchangeName);
            }
        }

        await unitOfWork.SaveAsync();
    }
}
