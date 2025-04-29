using InfoSymbolServer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoSymbolServer.Infrastructure.DataAccess;

/// <summary>
/// Represents the application's database context that provides access to the database entities
/// </summary>
/// <param name="options">The configuration options for this context</param>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Represents the collection of <see cref="Exchange"/> entities in the database.
    /// </summary>
    public DbSet<Exchange> Exchanges { get; set; }

    /// <summary>
    /// Represents the collection of <see cref="Symbol"/> entities in the database.
    /// </summary>
    public DbSet<Symbol> Symbols { get; set; }
    
    /// <summary>
    /// Represents the collection of <see cref="Status"/> entities in the database.
    /// </summary>
    public DbSet<Status> Statuses { get; set; }
    
    /// <summary>
    /// Represents the collection of <see cref="NotificationSettings"/> entities in the database.
    /// </summary>
    public DbSet<NotificationSettings> NotificationSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
