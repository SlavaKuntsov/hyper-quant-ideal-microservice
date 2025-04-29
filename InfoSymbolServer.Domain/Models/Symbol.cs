using InfoSymbolServer.Domain.Enums;

namespace InfoSymbolServer.Domain.Models;

/// <summary>
/// Represents a trading instrument (symbol) on an exchange.
/// </summary>
public class Symbol
{
    /// <summary>
    /// Unique identifier for the symbol.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifier of the exchange this symbol belongs to.
    /// </summary>
    public Guid ExchangeId { get; set; }

    /// <summary>
    /// Name of the symbol (e.g., "BTCUSDT")
    /// </summary>
    public string SymbolName { get; set; } = string.Empty;

    /// <summary>
    /// Type of market this symbol operates
    /// </summary>
    public MarketType MarketType { get; set; }

    /// <summary>
    /// Base asset of the symbol (e.g., "BTC" in "BTCUSDT").
    /// </summary>
    public string BaseAsset { get; set; } = string.Empty;

    /// <summary>
    /// Quote asset of the symbol (e.g., "USDT" in "BTCUSDT").
    /// </summary>
    public string QuoteAsset { get; set; } = string.Empty;

    /// <summary>
    /// Current trading status of the symbol
    /// </summary>
    public SymbolStatus Status { get; set; }

    /// <summary>
    /// Number of decimal places allowed for the price of this symbol.
    /// </summary>
    public int PricePrecision { get; set; }

    /// <summary>
    /// GNumber of decimal places allowed for the quantity of this symbol.
    /// </summary>
    public int QuantityPrecision { get; set; }

    /// <summary>
    /// Type of contract for this symbol
    /// </summary>
    public ContractType? ContractType { get; set; }

    /// <summary>
    /// Delivery date for futures contracts
    /// </summary>
    public DateTime? DeliveryDate { get; set; }

    /// <summary>
    /// Asset used for margin (e.g., "USDT" for USDT-margined futures)
    /// </summary>
    public string? MarginAsset { get; set; }

    /// <summary>
    /// Minimum quantity allowed for an order of this symbol
    /// </summary>
    public decimal MinQuantity { get; set; }

    /// <summary>
    /// Minimum notional value (price * quantity) allowed for an order of this symbol
    /// </summary>
    public decimal MinNotional { get; set; }

    /// <summary>
    /// Maximum quantity allowed for an order of this symbol
    /// </summary>
    public decimal MaxQuantity { get; set; }

    /// <summary>
    /// Date and time when the symbol was updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    public Exchange? Exchange { get; set; }
}
