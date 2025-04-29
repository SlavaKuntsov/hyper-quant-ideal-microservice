using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Presentation.Requests;

namespace InfoSymbolServer.FunctionalTests.Tests;

public class ExchangeControllerTests(TestWebApplicationFactory factory) : FunctionalTestBase(factory)
{
    private const string BaseUrl = "/api/v1/exchanges";

    #region GetAll

    [Fact]
    public async Task GetAll_Should_ReturnOkWithExchanges_WhenExchangesExist()
    {
        // Arrange
        await TestDataFactory.CreateExchangesAsync(3);
        
        // Act
        var response = await HttpClient.GetAsync(BaseUrl);
        var result = await response.Content.ReadFromJsonAsync<List<ExchangeDto>>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }
    
    [Fact]
    public async Task GetAll_Should_ReturnOkWithEmptyList_WhenNoExchangesExist()
    {
        // Act
        var response = await HttpClient.GetAsync(BaseUrl);
        var result = await response.Content.ReadFromJsonAsync<List<ExchangeDto>>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetByName
    
    [Fact]
    public async Task GetByName_Should_ReturnOkWithExchange_WhenExchangeExists()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("TestExchange");
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}");
        var result = await response.Content.ReadFromJsonAsync<ExchangeDto>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Name.Should().Be(exchange.Name);
    }
    
    [Fact]
    public async Task GetByName_Should_ReturnNotFound_WhenExchangeDoesNotExist()
    {
        // Arrange
        var nonExistentName = "NonExistentExchange";
        
        // Act
        var response = await HttpClient.GetAsync($"{BaseUrl}/{nonExistentName}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_Should_ReturnCreatedAtActionWithExchange_WhenModelIsValid()
    {
        // Arrange
        var createRequest = TestDataFactory.CreateExchangeRequest();
        
        // Act
        var response = await PostJsonAsync(BaseUrl, createRequest);
        var result = await response.Content.ReadFromJsonAsync<ExchangeDto>(JsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.Name.Should().Be(createRequest.Name);
        response.Headers.Location.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Create_Should_ReturnBadRequest_WhenRequestInvalid()
    {
        // Arrange
        var invalidRequest = new CreateExchangeRequest
        {
            // Invalid: Name is required
            Name = ""
        };
        
        // Act
        var response = await PostJsonAsync(BaseUrl, invalidRequest);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_Should_ReturnBadRequest_WhenNameNotUnique()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync("UniqueExchangeName");
        var createRequest = TestDataFactory.CreateExchangeRequest("UniqueExchangeName");
        
        // Act
        var response = await PostJsonAsync(BaseUrl, createRequest);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Delete
    
    [Fact]
    public async Task Delete_Should_ReturnNoContent_WhenExchangeExists()
    {
        // Arrange
        var exchange = await TestDataFactory.CreateExchangeAsync();
        
        // Act
        var response = await DeleteAsync($"{BaseUrl}/{exchange.Name}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await HttpClient.GetAsync($"{BaseUrl}/{exchange.Name}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Delete_Should_ReturnNotFound_WhenExchangeDoesNotExist()
    {
        // Arrange
        var nonExistentName = "NonExistentExchange";
        
        // Act
        var response = await DeleteAsync($"{BaseUrl}/{nonExistentName}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
