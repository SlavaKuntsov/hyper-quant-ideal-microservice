namespace InfoSymbolServer.Domain.Enums;

/// <summary>
/// Represents the different types of markets available for trading
/// </summary>
public enum MarketType
{
    /// <summary>
    /// Spot trading market where assets are traded for immediate delivery
    /// </summary>
    Spot,
    
    /// <summary>
    /// USDT-margined futures contracts market (denominated in USDT)
    /// </summary>
    UsdtFutures,
    
    /// <summary>
    /// Coin-margined futures contracts market (denominated in cryptocurrency)
    /// </summary>
    CoinFutures
}
