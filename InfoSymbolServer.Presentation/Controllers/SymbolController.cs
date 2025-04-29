using AutoMapper;
using InfoSymbolServer.Application.Abstractions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Presentation.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoSymbolServer.Presentation.Controllers;

/// <summary>
/// API Controller for managing trading symbols
/// </summary>
[ApiController]
[Route("api/v1/exchanges")]
[Produces("application/json")]
[Tags("Symbols")]
public class SymbolController : ControllerBase
{
    private readonly ISymbolService _symbolService;
    private readonly IMapper _mapper;

    public SymbolController(ISymbolService symbolService, IMapper mapper)
    {
        _symbolService = symbolService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all symbols for an exchange.
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A collection of symbols.</returns>
    /// <response code="200">Returns the list of symbols</response>
    [HttpGet("{exchangeName}/symbols")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SymbolDto>))]
    public async Task<ActionResult<IEnumerable<SymbolDto>>> GetAll(
        [FromRoute] string exchangeName,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var symbols = await _symbolService.GetForExchangeAsync(
            exchangeName, pageNumber, pageSize, cancellationToken);
        return Ok(symbols);
    }

    /// <summary>
    /// Gets all symbols for an exchange without pagination.
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A collection of all symbols for the exchange.</returns>
    /// <response code="200">Returns the complete list of symbols</response>
    [HttpGet("{exchangeName}/symbols-list")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SymbolDto>))]
    public async Task<ActionResult<IEnumerable<SymbolDto>>> GetAllList(
        [FromRoute] string exchangeName,
        CancellationToken cancellationToken = default)
    {
        var symbols = await _symbolService.GetForExchangeAsync(
            exchangeName, cancellationToken: cancellationToken);
        return Ok(symbols);
    }

    /// <summary>
    /// Gets all active symbols for an exchange.
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A collection of active symbols.</returns>
    /// <response code="200">Returns the list of active symbols</response>
    [HttpGet("{exchangeName}/active-symbols")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SymbolDto>))]
    public async Task<ActionResult<IEnumerable<SymbolDto>>> GetActive(
        [FromRoute] string exchangeName,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var symbols = await _symbolService.GetActiveForExchangeAsync(
            exchangeName, pageNumber, pageSize, cancellationToken);
        return Ok(symbols);
    }

    /// <summary>
    /// Gets all active symbols for an exchange without pagination.
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A collection of all active symbols for the exchange.</returns>
    /// <response code="200">Returns the complete list of active symbols</response>
    [HttpGet("{exchangeName}/active-symbols-list")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SymbolDto>))]
    public async Task<ActionResult<IEnumerable<SymbolDto>>> GetActiveList(
        [FromRoute] string exchangeName,
        CancellationToken cancellationToken = default)
    {
        var symbols = await _symbolService.GetActiveForExchangeAsync(
            exchangeName, cancellationToken: cancellationToken);
        return Ok(symbols);
    }

    /// <summary>
    /// Gets a symbol by its name for a specific exchange.
    /// </summary>
    /// <param name="exchangeName">The exchange name.</param>
    /// <param name="symbolName">The symbol name.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The symbol with the specified name on the specified exchange.</returns>
    /// <response code="200">Returns the symbol</response>
    /// <response code="404">If the symbol is not found</response>
    [HttpGet("{exchangeName}/symbols/{symbolName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SymbolDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SymbolDto>> GetByName(
        [FromRoute] string exchangeName,
        [FromRoute] string symbolName,
        CancellationToken cancellationToken = default)
    {
        var symbol = await _symbolService.GetForExchangeByNameAsync(
            symbolName, exchangeName, cancellationToken);
        return Ok(symbol);
    }
    
    /// <summary>
    /// Adds a new symbol.
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="request">The symbol data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The added symbol</returns>
    /// <response code="200">Returns the added symbol</response>
    /// <response code="400">If the symbol already exists</response>
    /// <response code="404">If the exchange is not found</response>
    [HttpPost("{exchangeName}/symbols")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SymbolDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SymbolDto>> Add(
        [FromRoute] string exchangeName,
        [FromBody] AddSymbolRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = _mapper.Map<AddSymbolDto>(request);
        dto.ExchangeName = exchangeName;
        
        var result = await _symbolService.AddAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Revokes a symbol's deleted status by changing its status from RemovedByAdmin to AddedByAdmin.
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="symbolName">The symbol name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated symbol with AddedByAdmin status</returns>
    /// <response code="200">Returns the updated symbol</response>
    /// <response code="400">If the symbol is not in RemovedByAdmin status</response>
    /// <response code="404">If the symbol or exchange is not found</response>
    [HttpPost("{exchangeName}/symbols/{symbolName}/revoke")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SymbolDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SymbolDto>> RevokeDelete(
        [FromRoute] string exchangeName,
        [FromRoute] string symbolName,
        CancellationToken cancellationToken = default)
    {
        var result = await _symbolService.RevokeDeleteAsync(symbolName, exchangeName, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Marks a symbol as deleted by admin (sets status to RemovedByAdmin).
    /// </summary>
    /// <param name="exchangeName">The exchange name</param>
    /// <param name="symbolName">The symbol name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated symbol with RemovedByAdmin status</returns>
    /// <response code="204">No content on success</response>
    /// <response code="404">If the symbol or exchange is not found</response>
    [HttpDelete("{exchangeName}/symbols/{symbolName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(
        [FromRoute] string exchangeName,
        [FromRoute] string symbolName,
        CancellationToken cancellationToken = default)
    {
        await _symbolService.DeleteAsync(symbolName, exchangeName, cancellationToken);
        return NoContent();
    }
}
