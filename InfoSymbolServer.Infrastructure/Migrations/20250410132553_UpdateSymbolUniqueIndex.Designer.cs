﻿// <auto-generated />
using System;
using System.Collections.Generic;
using InfoSymbolServer.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InfoSymbolServer.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250410132553_UpdateSymbolUniqueIndex")]
    partial class UpdateSymbolUniqueIndex
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("InfoSymbolServer.Domain.Models.Exchange", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<bool>("IsFutures")
                        .HasColumnType("boolean")
                        .HasColumnName("is_futures");

                    b.Property<bool>("IsOptions")
                        .HasColumnType("boolean")
                        .HasColumnName("is_options");

                    b.Property<bool>("IsSpot")
                        .HasColumnType("boolean")
                        .HasColumnName("is_spot");

                    b.Property<decimal?>("MinTradeSize")
                        .HasColumnType("numeric")
                        .HasColumnName("min_trade_size");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.PrimitiveCollection<List<string>>("OrderTypes")
                        .IsRequired()
                        .HasColumnType("TEXT[]")
                        .HasColumnName("order_types");

                    b.HasKey("Id")
                        .HasName("pk_exchanges");

                    b.ToTable("exchanges", (string)null);
                });

            modelBuilder.Entity("InfoSymbolServer.Domain.Models.Symbol", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("BaseAsset")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("base_asset");

                    b.Property<string>("ContractType")
                        .HasColumnType("text")
                        .HasColumnName("contract_type");

                    b.Property<DateTime?>("DeliveryDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("delivery_date");

                    b.Property<Guid>("ExchangeId")
                        .HasColumnType("uuid")
                        .HasColumnName("exchange_id");

                    b.Property<string>("MarginAsset")
                        .HasColumnType("text")
                        .HasColumnName("margin_asset");

                    b.Property<string>("MarketType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("market_type");

                    b.Property<decimal>("MaxQuantity")
                        .HasColumnType("numeric")
                        .HasColumnName("max_quantity");

                    b.Property<decimal>("MinNotional")
                        .HasColumnType("numeric")
                        .HasColumnName("min_notional");

                    b.Property<decimal>("MinQuantity")
                        .HasColumnType("numeric")
                        .HasColumnName("min_quantity");

                    b.Property<int>("PricePrecision")
                        .HasColumnType("integer")
                        .HasColumnName("price_precision");

                    b.Property<int>("QuantityPrecision")
                        .HasColumnType("integer")
                        .HasColumnName("quantity_precision");

                    b.Property<string>("QuoteAsset")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("quote_asset");

                    b.Property<bool>("ShouldSynchronize")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("should_synchronize");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<string>("SymbolName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("symbol_name");

                    b.HasKey("Id")
                        .HasName("pk_symbols");

                    b.HasIndex("ExchangeId", "SymbolName", "MarketType")
                        .IsUnique()
                        .HasDatabaseName("ix_symbols_exchange_id_symbol_name_market_type");

                    b.ToTable("symbols", (string)null);
                });

            modelBuilder.Entity("InfoSymbolServer.Domain.Models.Symbol", b =>
                {
                    b.HasOne("InfoSymbolServer.Domain.Models.Exchange", "Exchange")
                        .WithMany("Symbols")
                        .HasForeignKey("ExchangeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_symbols_exchanges_exchange_id");

                    b.Navigation("Exchange");
                });

            modelBuilder.Entity("InfoSymbolServer.Domain.Models.Exchange", b =>
                {
                    b.Navigation("Symbols");
                });
#pragma warning restore 612, 618
        }
    }
}
