using InfoSymbolServer.Domain.Enums;

namespace InfoSymbolServer.Domain.Models;

/// <summary>
/// Represents an exchange (trading platform)
/// </summary>
public class Exchange
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the exchange
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Date and time when the exchange was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Collection of symbols available on this exchange
    /// </summary>
    public ICollection<Symbol> Symbols { get; set; } = [];
}
