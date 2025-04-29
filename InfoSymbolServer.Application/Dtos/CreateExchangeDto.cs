using InfoSymbolServer.Domain.Enums;

namespace InfoSymbolServer.Application.Dtos;

/// <summary>
/// Data Transfer Object for creating a new Exchange
/// </summary>
public record CreateExchangeDto
{
    /// <summary>
    /// Name of the exchange
    /// </summary>
    public string Name { get; init; } = null!;
}
