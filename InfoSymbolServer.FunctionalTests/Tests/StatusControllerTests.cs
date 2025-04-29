using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Domain.Enums;

namespace InfoSymbolServer.FunctionalTests.Tests;

public class StatusControllerTests(TestWebApplicationFactory factory) : FunctionalTestBase(factory)
{
    private const string BaseUrl = "/api/v1/exchanges";

    #region GetExchangeSymbolsHistory

    [Fact]
    public async Task GetExchangeSymbolsHistory_Should_ReturnOkWithHistory_WhenSymbolsWithHistoryExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        var symbol1 = await TestDataFactory.CreateSymbolAsync(exchange.Id, "BTC/USDT");
        var symbol2 = await TestDataFactory.CreateSymbolAsync(exchange.Id, "ETH/USDT");
        var histories1 = await TestDataFactory.CreateSymbolStatusHistoryAsync(symbol1.Id, 3);
        var histories2 = await TestDataFactory.CreateSymbolStatusHistoryAsync(symbol2.Id, 2);
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}/symbols/history");
        var result = await response.Content.ReadFromJsonAsync<List<SymbolHistoryDto>>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        
        var btcHistory = result.FirstOrDefault(h => h.SymbolName == "BTC/USDT");
        btcHistory.Should().NotBeNull();
        btcHistory!.History.Should().HaveCount(3);
        
        var ethHistory = result.FirstOrDefault(h => h.SymbolName == "ETH/USDT");
        ethHistory.Should().NotBeNull();
        ethHistory!.History.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task GetExchangeSymbolsHistory_Should_ReturnOkWithEmptyList_WhenNoSymbolsExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}/symbols/history");
        var result = await response.Content.ReadFromJsonAsync<List<SymbolHistoryDto>>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetExchangeSymbolsHistory_Should_ReturnNotFound_WhenExchangeDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/NonExistentExchange/symbols/history");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GetExchangeActiveSymbolsHistory

    [Fact]
    public async Task GetExchangeActiveSymbolsHistory_Should_ReturnOkWithHistory_WhenActiveSymbolsWithHistoryExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        var activeSymbol = await TestDataFactory.CreateSymbolAsync(exchange.Id, "BTC/USDT", SymbolStatus.Active);
        var inactiveSymbol = await TestDataFactory.CreateSymbolAsync(exchange.Id, "ETH/USDT", SymbolStatus.Suspended);
        var activeHistories = await TestDataFactory.CreateSymbolStatusHistoryAsync(activeSymbol.Id, 3);
        var inactiveHistories = await TestDataFactory.CreateSymbolStatusHistoryAsync(inactiveSymbol.Id, 2);
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}/active-symbols/history");
        var result = await response.Content.ReadFromJsonAsync<List<SymbolHistoryDto>>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        
        var activeHistory = result.FirstOrDefault(h => h.SymbolName == "BTC/USDT");
        activeHistory.Should().NotBeNull();
        activeHistory!.History.Should().HaveCount(3);
    }
    
    [Fact]
    public async Task GetExchangeActiveSymbolsHistory_Should_ReturnOkWithEmptyList_WhenNoActiveSymbolsExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        var inactiveSymbol = await TestDataFactory.CreateSymbolAsync(exchange.Id, "ETH/USDT", SymbolStatus.Suspended);
        var inactiveHistories = await TestDataFactory.CreateSymbolStatusHistoryAsync(inactiveSymbol.Id, 2);
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}/active-symbols/history");
        var result = await response.Content.ReadFromJsonAsync<List<SymbolHistoryDto>>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetExchangeActiveSymbolsHistory_Should_ReturnNotFound_WhenExchangeDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/NonExistentExchange/active-symbols/history");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
    
    #region GetSymbolHistory
    
    [Fact]
    public async Task GetSymbolHistory_Should_ReturnOkWithHistory_WhenSymbolWithHistoryExists()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        var symbol = await TestDataFactory.CreateSymbolAsync(exchange.Id, "BTC/USDT");
        var histories = await TestDataFactory.CreateSymbolStatusHistoryAsync(symbol.Id, 3);
        
        // Act
        var response = await HttpClient.GetAsync(
            $"{BaseUrl}/{exchange.Name}/symbols/{Uri.EscapeDataString(symbol.SymbolName)}/history");
        var result = await response.Content.ReadFromJsonAsync<SymbolHistoryDto>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.SymbolName.Should().Be(symbol.SymbolName);
        result.History.Should().HaveCount(3);
    }
    
    [Fact]
    public async Task GetSymbolHistory_Should_ReturnNotFound_WhenSymbolDoesNotExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}/symbols/NonExistentSymbol/history");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetSymbolHistory_Should_ReturnNotFound_WhenExchangeDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/NonExistentExchange/symbols/BTC-USDT/history");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    #endregion
} 