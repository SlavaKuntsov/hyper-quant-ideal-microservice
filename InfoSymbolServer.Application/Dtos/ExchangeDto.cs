using InfoSymbolServer.Domain.Enums;

namespace InfoSymbolServer.Application.Dtos;

/// <summary>
/// Data Transfer Object for Exchange
/// </summary>
public record ExchangeDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Name of the exchange
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Date and time when the exchange was created
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
