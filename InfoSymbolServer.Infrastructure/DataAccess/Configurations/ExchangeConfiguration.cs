using InfoSymbolServer.Domain.Enums;
using InfoSymbolServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InfoSymbolServer.Infrastructure.DataAccess.Configurations;

/// <summary>
/// Provides entity type configuration for the <see cref="Exchange"/> entity.
/// Defines the database schema, relationships, and constraints for Exchange records.
/// </summary>
public class ExchangeConfiguration : IEntityTypeConfiguration<Exchange>
{
    public void Configure(EntityTypeBuilder<Exchange> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.HasMany(e => e.Symbols)
            .WithOne(s => s.Exchange)
            .HasForeignKey(s => s.ExchangeId);
            
        builder.HasIndex(e => e.Name)
            .IsUnique();
    }
}
