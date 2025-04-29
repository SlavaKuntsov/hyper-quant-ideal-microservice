using InfoSymbolServer.Domain.Enums;

namespace InfoSymbolServer.Domain.Models;

/// <summary>
/// Represents the status history of a trading symbol.
/// </summary>
public class Status
{
    /// <summary>
    /// Unique identifier for the status record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Date and time when this status was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The trading status of the symbol at this point in time.
    /// </summary>
    public SymbolStatus SymbolStatus { get; set; }

    /// <summary>
    /// Identifier of the symbol this status belongs to.
    /// </summary>
    public Guid SymbolId { get; set; }

    /// <summary>
    /// Navigation property to the associated symbol.
    /// </summary>
    public Symbol? Symbol { get; set; }
}
