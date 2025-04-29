namespace InfoSymbolServer.Domain.Enums;

/// <summary>
/// Represents the status of a trading symbol on an exchange.
/// </summary>
public enum SymbolStatus
{
    /// <summary>
    /// The symbol is actively trading on the exchange without any restrictions.
    /// </summary>
    Active,

    /// <summary>
    /// Trading for this symbol has been temporarily suspended for a finite period (not permanently).
    /// </summary>
    Suspended,
    
    /// <summary>
    /// The symbol is in a pre-trading phase, preceding the active trading stage.
    /// </summary>
    PreLaunch,
    
    /// <summary>
    /// The symbol has been permanently removed from the exchange.
    /// </summary>
    Delisted,
    
    /// <summary>
    /// Relevant for time-limited instruments, such as quarterly futures,
    /// indicating the instrument has reached its expiration date.
    /// </summary>
    Expired,
    
    /// <summary>
    /// This status may precede the Expired status for certain instruments,
    /// typically indicating the instrument is in its settlement phase.
    /// </summary>
    Settling,
    
    /// <summary>
    /// The symbol was manually added by an administrator and has not yet
    /// been synchronized with the exchange.
    /// </summary>
    AddedByAdmin,
    
    /// <summary>
    /// The symbol was removed by an administrator and is no longer being
    /// synchronized with the exchange, although it may still be trading there.
    /// </summary>
    RemovedByAdmin
}
