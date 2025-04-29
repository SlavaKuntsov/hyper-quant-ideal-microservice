using InfoSymbolServer.Application.Abstractions;
using InfoSymbolServer.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoSymbolServer.Presentation.Controllers;

/// <summary>
/// API Controller for managing symbol status history
/// </summary>
[ApiController]
[Route("api/v1/exchanges")]
[Produces("application/json")]
[Tags("Status History")]
public class StatusController : ControllerBase
{
    private readonly IStatusService _statusService;

    public StatusController(IStatusService statusService)
    {
        _statusService = statusService;
    }

    /// <summary>
    /// Gets status history for all symbols in an exchange
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Status history for all symbols in the exchange</returns>
    /// <response code="200">Returns the status history</response>
    /// <response code="404">If the exchange is not found</response>
    [HttpGet("{exchangeName}/symbols/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<SymbolHistoryDto>>> GetExchangeSymbolsHistory(
        [FromRoute] string exchangeName,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var history = await _statusService.GetExchangeSymbolsHistoryAsync(
            exchangeName, 
            pageNumber, 
            pageSize, 
            cancellationToken);
            
        return Ok(history);
    }

    /// <summary>
    /// Gets status history for all symbols in an exchange without pagination
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Status history for all symbols in the exchange</returns>
    /// <response code="200">Returns the complete status history</response>
    /// <response code="404">If the exchange is not found</response>
    [HttpGet("{exchangeName}/symbols-list/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<SymbolHistoryDto>>> GetExchangeSymbolsHistoryList(
        [FromRoute] string exchangeName,
        CancellationToken cancellationToken = default)
    {
        var history = await _statusService.GetExchangeSymbolsHistoryAsync(
            exchangeName, cancellationToken: cancellationToken);
            
        return Ok(history);
    }

    /// <summary>
    /// Gets status history for active symbols in an exchange
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Status history for active symbols in the exchange</returns>
    /// <response code="200">Returns the status history</response>
    /// <response code="404">If the exchange is not found</response>
    [HttpGet("{exchangeName}/active-symbols/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<SymbolHistoryDto>>> GetExchangeActiveSymbolsHistory(
        [FromRoute] string exchangeName,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var history = await _statusService.GetExchangeActiveSymbolsHistoryAsync(
            exchangeName, 
            pageNumber, 
            pageSize, 
            cancellationToken);
            
        return Ok(history);
    }

    /// <summary>
    /// Gets status history for active symbols in an exchange without pagination
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Status history for all active symbols in the exchange</returns>
    /// <response code="200">Returns the complete status history for active symbols</response>
    /// <response code="404">If the exchange is not found</response>
    [HttpGet("{exchangeName}/active-symbols-list/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<SymbolHistoryDto>>> GetExchangeActiveSymbolsHistoryList(
        [FromRoute] string exchangeName,
        CancellationToken cancellationToken = default)
    {
        var history = await _statusService.GetExchangeActiveSymbolsHistoryAsync(
            exchangeName, cancellationToken: cancellationToken);
            
        return Ok(history);
    }

    /// <summary>
    /// Gets status history for a specific symbol in an exchange
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="symbolName">The symbol name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Status history for the symbol</returns>
    /// <response code="200">Returns the status history</response>
    /// <response code="404">If the exchange or symbol is not found</response>
    [HttpGet("{exchangeName}/symbols/{symbolName}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SymbolHistoryDto>> GetSymbolHistory(
        [FromRoute] string exchangeName,
        [FromRoute] string symbolName,
        CancellationToken cancellationToken = default)
    {
        var history = await _statusService.GetSymbolHistoryAsync(
            symbolName, 
            exchangeName, 
            cancellationToken);
            
        return Ok(history);
    }
}
