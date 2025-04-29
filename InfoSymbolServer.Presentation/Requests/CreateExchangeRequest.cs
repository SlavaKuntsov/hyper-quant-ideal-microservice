using InfoSymbolServer.Domain.Enums;

namespace InfoSymbolServer.Presentation.Requests;

/// <summary>
/// Request model for creating a new exchange
/// </summary>
public record CreateExchangeRequest
{
    /// <summary>
    /// Name of the exchange
    /// </summary>
    public string Name { get; init; } = null!;
}
