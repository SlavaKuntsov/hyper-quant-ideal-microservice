using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using InfoSymbolServer.Infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace InfoSymbolServer.FunctionalTests;

/// <summary>
/// Base class for functional tests class.
/// </summary>
public abstract class FunctionalTestBase : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient HttpClient;
    protected readonly TestDataFactory TestDataFactory;
    protected readonly JsonSerializerOptions JsonOptions;
    private readonly IServiceScope _scope;
    private readonly ISchedulerFactory _schedulerFactory;

    protected FunctionalTestBase(TestWebApplicationFactory factory)
    {
        HttpClient = factory.CreateClient();
        
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        JsonOptions.Converters.Add(new JsonStringEnumConverter());

        _scope = factory.Services.CreateScope();
        _schedulerFactory = _scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();

        var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        TestDataFactory = new TestDataFactory(dbContext);
    }

    /// <summary>
    /// Cleans up the database and set Quartz scheduler to standby while testing.
    /// </summary>
    public async Task InitializeAsync()
    {
        // Clean the database
        await TestDataFactory.CleanDatabaseAsync();

        // Ensure Quartz scheduler is in standby mode for tests
        var scheduler = await _schedulerFactory.GetScheduler();

        // If scheduler is running, put it in standby for tests
        if (!scheduler.InStandbyMode)
        {
            await scheduler.Standby();
        }
    }

    /// <summary>
    /// Cleans up dependencies after testing.
    /// </summary>
    public async Task DisposeAsync()
    {
        _scope.Dispose();
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.Standby();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Serializes data into json and sends HTTP POST request.
    /// </summary>
    /// <param name="url">Request url</param>
    /// <param name="data">Data to serialize</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected async Task<HttpResponseMessage> PostJsonAsync<T>(string url, T data)
    {
        return await HttpClient.PostAsJsonAsync(url, data, JsonOptions);
    }

    /// <summary>
    /// Serializes data into json and sends HTTP PUT request.
    /// </summary>
    /// <param name="url">Request url</param>
    /// <param name="data">Data to serialize</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected async Task<HttpResponseMessage> PutJsonAsync<T>(string url, T data)
    {
        return await HttpClient.PutAsJsonAsync(url, data, JsonOptions);
    }

    /// <summary>
    /// Sends delete request for specified url.
    /// </summary>
    /// <param name="url">Request url</param>
    /// <returns></returns>
    protected async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        return await HttpClient.DeleteAsync(url);
    }
}
