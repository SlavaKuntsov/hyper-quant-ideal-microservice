using InfoSymbolServer.Domain.Enums;

namespace InfoSymbolServer.Application.Dtos;

/// <summary>
/// Data Transfer Object for Symbol Status history
/// </summary>
public record StatusDto
{
    /// <summary>
    /// Date and time when this status was updated.
    /// </summary>
    public DateTime UpdatedAt { get; init; }

    /// <summary>
    /// The trading status of the symbol at this point in time.
    /// </summary>
    public SymbolStatus SymbolStatus { get; init; }
} 
