namespace InfoSymbolServer.Application.Dtos;

/// <summary>
/// Data Transfer Object for Symbol Status history response
/// </summary>
public record SymbolHistoryDto
{
    /// <summary>
    /// The name of the symbol
    /// </summary>
    public string SymbolName { get; init; } = null!;
    
    /// <summary>
    /// The history of status changes for this symbol
    /// </summary>
    public IEnumerable<StatusDto> History { get; init; } = [];
}
