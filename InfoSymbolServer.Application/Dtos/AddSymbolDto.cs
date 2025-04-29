using InfoSymbolServer.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace InfoSymbolServer.Application.Dtos;

/// <summary>
/// Data Transfer Object for adding a symbol by an administrator
/// </summary>
public record AddSymbolDto
{
    /// <summary>
    /// The exchange name this symbol belongs to
    /// </summary>
    public string ExchangeName { get; set; } = null!;
    
    /// <summary>
    /// Name of the symbol (e.g., "BTCUSDT")
    /// </summary>
    public string SymbolName { get; init; } = null!;

    /// <summary>
    /// Type of market this symbol operates
    /// </summary>
    public MarketType MarketType { get; init; }

    /// <summary>
    /// Base asset of the symbol (e.g., "BTC" in "BTCUSDT")
    /// </summary>
    public string BaseAsset { get; init; } = null!;

    /// <summary>
    /// Quote asset of the symbol (e.g., "USDT" in "BTCUSDT")
    /// </summary>
    public string QuoteAsset { get; init; } = null!;

    /// <summary>
    /// Number of decimal places allowed for the price of this symbol
    /// </summary>
    public int PricePrecision { get; init; }

    /// <summary>
    /// Number of decimal places allowed for the quantity of this symbol
    /// </summary>
    public int QuantityPrecision { get; init; }

    /// <summary>
    /// Type of contract for this symbol
    /// </summary>
    public ContractType? ContractType { get; init; }

    /// <summary>
    /// Delivery date for futures contracts
    /// </summary>
    public DateTime? DeliveryDate { get; init; }

    /// <summary>
    /// Asset used for margin (e.g., "USDT" for USDT-margined futures)
    /// </summary>
    public string MarginAsset { get; init; } = null!;

    /// <summary>
    /// Minimum quantity allowed for an order of this symbol
    /// </summary>
    public decimal MinQuantity { get; init; }

    /// <summary>
    /// Minimum notional value (price * quantity) allowed for an order of this symbol
    /// </summary>
    public decimal MinNotional { get; init; }

    /// <summary>
    /// Maximum quantity allowed for an order of this symbol
    /// </summary>
    public decimal MaxQuantity { get; init; }
}
