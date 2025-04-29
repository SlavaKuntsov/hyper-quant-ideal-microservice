using InfoSymbolServer.Domain.Enums;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Infrastructure.DataAccess;
using InfoSymbolServer.Presentation.Requests;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Domain.Constants;

namespace InfoSymbolServer.FunctionalTests;

/// <summary>
/// Helper class to seed the database with test data.
/// </summary>
/// <param name="dbContext"></param>
public class TestDataFactory(ApplicationDbContext dbContext)
{
    /// <summary>
    /// Creates exchange object and sends it into database
    /// </summary>
    /// <param name="name">Exchange name</param>
    /// <returns>Created object</returns>
    public async Task<Exchange> CreateExchangeAsync(string name = "TestExchange")
    {
        var exchange = new Exchange
        {
            Id = Guid.NewGuid(),
            Name = name
        };
        
        dbContext.Exchanges.Add(exchange);
        await dbContext.SaveChangesAsync();
        
        return exchange;
    }
    
    /// <summary>
    /// Creates specified count of exchanges.
    /// </summary>
    /// <param name="count">Count of exchanges</param>
    /// <returns>List of created exchanges</returns>
    public async Task<List<Exchange>> CreateExchangesAsync(int count)
    {
        var exchanges = new List<Exchange>();
        
        for (int i = 0; i < count; i++)
        {
            exchanges.Add(await CreateExchangeAsync($"TestExchange{i}"));
        }
        
        return exchanges;
    }
    
    /// <summary>
    /// Creates symbol and saves it to database
    /// </summary>
    /// <param name="exchangeId">Exchange ID</param>
    /// <param name="symbolName">Name of the symbol</param>
    /// <param name="status">Symbol's status</param>
    /// <returns></returns>
    public async Task<Symbol> CreateSymbolAsync(
        Guid exchangeId, string symbolName = "BTC/USDT", SymbolStatus status = SymbolStatus.Active)
    {
        var symbol = new Symbol
        {
            Id = Guid.NewGuid(),
            ExchangeId = exchangeId,
            SymbolName = symbolName,
            BaseAsset = "BTC",
            QuoteAsset = "USDT",
            Status = status,
            MarketType = MarketType.Spot,
            ContractType = ContractType.Spot,
            PricePrecision = 2,
            QuantityPrecision = 6,
            MinQuantity = 0.001m,
            MaxQuantity = 1000m,
            MinNotional = 10m
        };
        
        dbContext.Symbols.Add(symbol);
        await dbContext.SaveChangesAsync();
        
        return symbol;
    }
    
    /// <summary>
    /// Creates specified count of symbols.
    /// </summary>
    /// <param name="exchangeId">Exchange ID</param>
    /// <param name="count">Count of symbols</param>
    /// <returns></returns>
    public async Task<List<Symbol>> CreateSymbolsAsync(Guid exchangeId, int count)
    {
        var symbols = new List<Symbol>();
        
        for (int i = 0; i < count; i++)
        {
            symbols.Add(await CreateSymbolAsync(exchangeId, $"BTC{i}/USDT"));
        }
        
        return symbols;
    }
    
    /// <summary>
    /// Creates symbol status history
    /// </summary>
    /// <param name="symbolId">Symbol ID</param>
    /// <param name="entries">Number of history entries to create</param>
    /// <returns>The list of created status history entries</returns>
    public async Task<List<Status>> CreateSymbolStatusHistoryAsync(Guid symbolId, int entries = 3)
    {
        var histories = new List<Status>();
        var statuses = new[] { SymbolStatus.Active, SymbolStatus.Suspended, SymbolStatus.PreLaunch, SymbolStatus.Delisted };
        
        var baseTime = DateTime.UtcNow.AddDays(-entries);
        
        for (int i = 0; i < entries; i++)
        {
            var status = new Status
            {
                Id = Guid.NewGuid(),
                SymbolId = symbolId,
                SymbolStatus = statuses[i % statuses.Length],
                CreatedAt = baseTime.AddDays(i)
            };
            
            dbContext.Statuses.Add(status);
            histories.Add(status);
        }
        
        await dbContext.SaveChangesAsync();
        return histories;
    }
    
    /// <summary>
    /// Creates CreateExchangeRequest object.
    /// </summary>
    /// <param name="name">The name of the exchange</param>
    /// <returns></returns>
    public CreateExchangeRequest CreateExchangeRequest(string name = SupportedExchanges.BinanceSpot)
    {
        return new CreateExchangeRequest
        {
            Name = name
        };
    }
    
    /// <summary>
    /// Creates notification settings and saves it to database
    /// </summary>
    /// <param name="isTelegramEnabled">Whether Telegram notifications are enabled</param>
    /// <param name="isEmailEnabled">Whether Email notifications are enabled</param>
    /// <returns>Created notification settings</returns>
    public async Task<NotificationSettings> CreateNotificationSettingsAsync(
        bool isTelegramEnabled = true, 
        bool isEmailEnabled = true)
    {
        var settings = new NotificationSettings
        {
            Id = Guid.NewGuid(),
            IsTelegramEnabled = isTelegramEnabled,
            IsEmailEnabled = isEmailEnabled
        };
        
        dbContext.NotificationSettings.Add(settings);
        await dbContext.SaveChangesAsync();
        
        return settings;
    }
    
    /// <summary>
    /// Creates UpdateNotificationSettingsDto object
    /// </summary>
    /// <param name="isTelegramEnabled">Whether Telegram notifications are enabled</param>
    /// <param name="isEmailEnabled">Whether Email notifications are enabled</param>
    /// <returns>Created DTO</returns>
    public UpdateNotificationSettingsDto CreateUpdateNotificationSettingsDto(
        bool isTelegramEnabled = true, 
        bool isEmailEnabled = true)
    {
        return new UpdateNotificationSettingsDto
        {
            IsTelegramEnabled = isTelegramEnabled,
            IsEmailEnabled = isEmailEnabled
        };
    }
    
    /// <summary>
    /// Creates UpdateNotificationSettingsRequest object
    /// </summary>
    /// <param name="isTelegramEnabled">Whether Telegram notifications are enabled</param>
    /// <param name="isEmailEnabled">Whether Email notifications are enabled</param>
    /// <returns>Created request</returns>
    public UpdateNotificationSettingsRequest CreateUpdateNotificationSettingsRequest(
        bool isTelegramEnabled = true, 
        bool isEmailEnabled = true)
    {
        return new UpdateNotificationSettingsRequest
        {
            IsTelegramEnabled = isTelegramEnabled,
            IsEmailEnabled = isEmailEnabled
        };
    }
    
    /// <summary>
    /// Remove all symbols, status histories and exchanges from the database.
    /// </summary>
    public async Task CleanDatabaseAsync()
    {
        dbContext.Statuses.RemoveRange(dbContext.Statuses);
        dbContext.Symbols.RemoveRange(dbContext.Symbols);
        dbContext.Exchanges.RemoveRange(dbContext.Exchanges);
        dbContext.NotificationSettings.RemoveRange(dbContext.NotificationSettings);
        await dbContext.SaveChangesAsync();
    }
}
