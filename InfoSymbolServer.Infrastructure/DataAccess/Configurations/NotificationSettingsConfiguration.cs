using InfoSymbolServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoSymbolServer.Infrastructure.DataAccess.Configurations;

/// <summary>
/// Provides entity type configuration for the <see cref="NotificationSettings"/> entity.
/// Defines the database schema, relationships, and constraints for NotificationSettings records.
/// </summary>
public class NotificationSettingsConfiguration : IEntityTypeConfiguration<NotificationSettings>
{
    public void Configure(EntityTypeBuilder<NotificationSettings> builder)
    {
        builder.HasKey(ns => ns.Id);

        builder.Property(ns => ns.IsTelegramEnabled)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(ns => ns.IsEmailEnabled)
            .IsRequired()
            .HasDefaultValue(true);
            
        // Seed initial data - notifications enabled by default
        builder.HasData(
            new NotificationSettings
            {
                Id = Guid.Parse("5a23149e-79cc-4fed-8533-c3b4415c2cdb"),
                IsTelegramEnabled = true,
                IsEmailEnabled = true
            }
        );
    }
} 