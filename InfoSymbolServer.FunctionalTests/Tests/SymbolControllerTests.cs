using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Domain.Enums;
using InfoSymbolServer.Presentation.Requests;

namespace InfoSymbolServer.FunctionalTests.Tests;

public class SymbolControllerTests(TestWebApplicationFactory factory) : FunctionalTestBase(factory)
{
    private const string BaseUrl = "/api/v1/exchanges";

    #region GetAll

    [Fact]
    public async Task GetAll_Should_ReturnOkWithSymbols_WhenSymbolsExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        var symbols = await TestDataFactory.CreateSymbolsAsync(exchange.Id, 3);
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}/symbols");
        var result = await response.Content.ReadFromJsonAsync<List<SymbolDto>>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }
    
    [Fact]
    public async Task GetAll_Should_ReturnOkWithEmptyList_WhenNoSymbolsExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}/symbols");
        var result = await response.Content.ReadFromJsonAsync<List<SymbolDto>>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetAll_Should_ReturnNotFound_WhenExchangeDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/NonExistentExchange/symbols");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GetByName
    
    [Fact]
    public async Task GetByName_Should_ReturnOkWithSymbol_WhenSymbolExists()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        var symbol = await TestDataFactory.CreateSymbolAsync(exchange.Id, "BTC/USDT");
        
        // Act
        var response = await HttpClient
            .GetAsync($"{BaseUrl}/{exchange.Name}/symbols/{Uri.EscapeDataString(symbol.SymbolName)}");
        var result = await response.Content.ReadFromJsonAsync<SymbolDto>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.SymbolName.Should().Be(symbol.SymbolName);
        result.BaseAsset.Should().Be("BTC");
        result.QuoteAsset.Should().Be("USDT");
        result.MarketType.Should().Be(MarketType.Spot);
    }
    
    [Fact]
    public async Task GetByName_Should_ReturnNotFound_WhenSymbolDoesNotExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}/symbols/NonExistentSymbol");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetByName_Should_ReturnNotFound_WhenExchangeDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/NonExistentExchange/symbols/BTC/USDT");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    #endregion

    #region GetActive

    [Fact]
    public async Task GetActive_Should_ReturnOkWithSymbols_WhenActiveSymbolsExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        var activeSymbol1 = await TestDataFactory.CreateSymbolAsync(exchange.Id, "BTC/USDT", SymbolStatus.Active);
        var activeSymbol2 = await TestDataFactory.CreateSymbolAsync(exchange.Id, "ETH/USDT", SymbolStatus.Active);
        var inactiveSymbol = await TestDataFactory.CreateSymbolAsync(exchange.Id, "XRP/USDT", SymbolStatus.Suspended);
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}/active-symbols");
        var result = await response.Content.ReadFromJsonAsync<List<SymbolDto>>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(s => s.Status == SymbolStatus.Active).Should().BeTrue();
    }
    
    [Fact]
    public async Task GetActive_Should_ReturnOkWithEmptyList_WhenNoActiveSymbolsExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        var inactiveSymbol = await TestDataFactory.CreateSymbolAsync(exchange.Id, "XRP/USDT", SymbolStatus.Suspended);
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}/active-symbols");
        var result = await response.Content.ReadFromJsonAsync<List<SymbolDto>>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetActive_Should_ReturnNotFound_WhenExchangeDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/NonExistentExchange/active-symbols");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    #endregion

    #region Add

    [Fact]
    public async Task Add_Should_ReturnOkWithSymbol_WhenRequestIsValid()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        var request = new AddSymbolRequest
        {
            SymbolName = "ETH/USDT",
            MarketType = MarketType.Spot,
            BaseAsset = "ETH",
            QuoteAsset = "USDT",
            PricePrecision = 2,
            QuantityPrecision = 6,
            ContractType = ContractType.Spot,
            MarginAsset = "USDT",
            MinQuantity = 0.001m,
            MaxQuantity = 1000m,
            MinNotional = 10m
        };
        
        // Act
        var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/{exchange.Name}/symbols", request, JsonOptions);
        var result = await response.Content.ReadFromJsonAsync<SymbolDto>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.SymbolName.Should().Be(request.SymbolName);
        result.BaseAsset.Should().Be(request.BaseAsset);
        result.QuoteAsset.Should().Be(request.QuoteAsset);
        result.MarketType.Should().Be(request.MarketType);
    }
    
    [Fact]
    public async Task Add_Should_ReturnBadRequest_WhenSymbolAlreadyExists()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        var existingSymbol = await TestDataFactory.CreateSymbolAsync(exchange.Id, "ETH/USDT");
        
        var request = new AddSymbolRequest
        {
            SymbolName = "ETH/USDT",
            MarketType = MarketType.Spot,
            BaseAsset = "ETH",
            QuoteAsset = "USDT",
            PricePrecision = 2,
            QuantityPrecision = 6,
            ContractType = ContractType.Spot,
            MarginAsset = "USDT",
            MinQuantity = 0.001m,
            MaxQuantity = 1000m,
            MinNotional = 10m
        };
        
        // Act
        var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/{exchange.Name}/symbols", request, JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Add_Should_ReturnNotFound_WhenExchangeDoesNotExist()
    {
        // Arrange
        var request = new AddSymbolRequest
        {
            SymbolName = "ETH/USDT",
            MarketType = MarketType.Spot,
            BaseAsset = "ETH",
            QuoteAsset = "USDT",
            PricePrecision = 2,
            QuantityPrecision = 6,
            ContractType = ContractType.Spot,
            MarginAsset = "USDT",
            MinQuantity = 0.001m,
            MaxQuantity = 1000m,
            MinNotional = 10m
        };
        
        // Act
        var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/NonExistentExchange/symbols", request, JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_Should_ReturnNoContent_WhenSymbolExists()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        // Use a simple symbol name without special characters
        var symbol = await TestDataFactory.CreateSymbolAsync(exchange.Id, "BTCUSDT");
        
        // Act
        var response = await HttpClient.DeleteAsync(
            $"{BaseUrl}/{exchange.Name}/symbols/{symbol.SymbolName}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify the symbol's status was changed
        var getResponse = await HttpClient.GetAsync(
            $"{BaseUrl}/{exchange.Name}/symbols/{symbol.SymbolName}");
        var result = await getResponse.Content.ReadFromJsonAsync<SymbolDto>(JsonOptions);
        
        result.Should().NotBeNull();
        result!.Status.Should().Be(SymbolStatus.RemovedByAdmin);
    }
    
    [Fact]
    public async Task Delete_Should_ReturnNotFound_WhenSymbolDoesNotExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        
        // Act
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{exchange.Name}/symbols/NonExistentSymbol");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Delete_Should_ReturnNotFound_WhenExchangeDoesNotExist()
    {
        // Act
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/NonExistentExchange/symbols/BTCUSDT");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region RevokeDelete

    [Fact]
    public async Task RevokeDelete_Should_ReturnOkWithSymbol_WhenSymbolIsDeleted()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        // Use a simple symbol name without special characters
        var symbol = await TestDataFactory.CreateSymbolAsync(exchange.Id, "BTCUSDT", SymbolStatus.RemovedByAdmin);
        
        // Act
        var response = await HttpClient.PostAsync(
            $"{BaseUrl}/{exchange.Name}/symbols/{symbol.SymbolName}/revoke", null);
        var result = await response.Content.ReadFromJsonAsync<SymbolDto>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.SymbolName.Should().Be(symbol.SymbolName);
        result.Status.Should().Be(SymbolStatus.AddedByAdmin);
    }
    
    [Fact]
    public async Task RevokeDelete_Should_ReturnBadRequest_WhenSymbolIsNotDeleted()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        // Use a simple symbol name without special characters
        var symbol = await TestDataFactory.CreateSymbolAsync(exchange.Id, "BTCUSDT", SymbolStatus.Active);
        
        // Act
        var response = await HttpClient.PostAsync(
            $"{BaseUrl}/{exchange.Name}/symbols/{symbol.SymbolName}/revoke", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task RevokeDelete_Should_ReturnNotFound_WhenSymbolDoesNotExist()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        
        // Act
        var response = await HttpClient.PostAsync(
            $"{BaseUrl}/{exchange.Name}/symbols/NonExistentSymbol/revoke", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task RevokeDelete_Should_ReturnNotFound_WhenExchangeDoesNotExist()
    {
        // Act
        var response = await HttpClient.PostAsync(
            $"{BaseUrl}/NonExistentExchange/symbols/BTCUSDT/revoke", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
