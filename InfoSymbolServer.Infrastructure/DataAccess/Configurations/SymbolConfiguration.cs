using InfoSymbolServer.Domain.Enums;
using InfoSymbolServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InfoSymbolServer.Infrastructure.DataAccess.Configurations;

/// <summary>
/// Provides entity type configuration for the <see cref="Symbol"/> entity.
/// Defines the database schema, relationships, and constraints for Symbol records.
/// </summary>
public class SymbolConfiguration : IEntityTypeConfiguration<Symbol>
{
    public void Configure(EntityTypeBuilder<Symbol> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.SymbolName)
            .IsRequired();

        builder.Property(s => s.MarketType)
            .IsRequired()
            .HasConversion(new EnumToStringConverter<MarketType>());

        builder.Property(s => s.BaseAsset)
            .IsRequired();

        builder.Property(s => s.QuoteAsset)
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion(new EnumToStringConverter<SymbolStatus>());

        builder.Property(s => s.ContractType)
            .HasConversion(new EnumToStringConverter<ContractType>());

        builder.Property(s => s.MinQuantity)
            .IsRequired();

        builder.Property(s => s.MinNotional)
            .IsRequired();

        builder.Property(s => s.MaxQuantity)
            .IsRequired();

        builder.HasIndex(s => new { s.ExchangeId, s.SymbolName, s.MarketType })
            .IsUnique();
    }
}
