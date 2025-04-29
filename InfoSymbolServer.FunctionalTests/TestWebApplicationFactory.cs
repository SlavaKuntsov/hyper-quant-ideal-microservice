using InfoSymbolServer.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using Testcontainers.PostgreSql;

namespace InfoSymbolServer.FunctionalTests;

/// <summary>
/// WebApplicationFactory to run application in-memory for testing purposes.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // PostgreSQL database container run in Docker.
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:alpine")
        .WithDatabase("infosymboldb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
        
    /// <summary>
    /// Configures web host replacing some of the dependencies for testing.
    /// </summary>
    /// <param name="builder"></param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Replaces ApplicationDbContext configuration with test container configuration.
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options
                    .UseNpgsql(_dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention();
            });

            // Changes quartz database connection to test container connection.
            services.AddQuartz(config =>
            {
                config.UsePersistentStore(options =>
                    options.UsePostgres(_dbContainer.GetConnectionString()));
            });
        });
    }

    /// <summary>
    /// Runs database container.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    /// <summary>
    /// Stops and disposes database container.
    /// </summary>
    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}
