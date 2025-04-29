using InfoSymbolServer.Domain.Enums;
using InfoSymbolServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InfoSymbolServer.Infrastructure.DataAcces.Configurations;

/// <summary>
/// Provides entity type configuration for the <see cref="Status"/> entity.
/// Defines the database schema, relationships, and constraints for Status records.
/// </summary>
public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.SymbolStatus)
            .IsRequired()
            .HasConversion(new EnumToStringConverter<SymbolStatus>());

        builder.HasOne(s => s.Symbol)
            .WithMany()
            .HasForeignKey(s => s.SymbolId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
