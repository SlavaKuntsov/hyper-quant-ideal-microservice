namespace InfoSymbolServer.Domain.Enums;

/// <summary>
/// Represents the type of contract for a trading symbol.
/// </summary>
public enum ContractType
{
    /// <summary>
    /// Spot trading instrument with immediate settlement.
    /// </summary>
    Spot,

    /// <summary>
    /// Perpetual futures contract with no expiration date.
    /// </summary>
    Perpetual,

    /// <summary>
    /// Futures contract expiring in the current quarter.
    /// </summary>
    CurrentQuarter,

    /// <summary>
    /// Futures contract expiring in the next quarter.
    /// </summary>
    NextQuarter
}
