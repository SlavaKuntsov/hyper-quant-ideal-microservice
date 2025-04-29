using InfoSymbolServer.Application.Abstractions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Presentation.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace InfoSymbolServer.Presentation.Controllers;

/// <summary>
/// API Controller for managing exchange data
/// </summary>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[Tags("Exchanges")]
public class ExchangeController(
    IExchangeService exchangeService, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Gets all exchanges.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A collection of exchanges.</returns>
    /// <response code="200">Returns the list of exchanges</response>
    [HttpGet("exchanges")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ExchangeDto>))]
    public async Task<ActionResult<IEnumerable<ExchangeDto>>> GetAll(
        CancellationToken cancellationToken = default)
    {
        var exchanges = await exchangeService.GetAllAsync(cancellationToken);
        return Ok(exchanges);
    }

    /// <summary>
    /// Gets all supported exchanges names.
    /// </summary>
    /// <returns>A collection of supported exchange names.</returns>
    /// <response code="200">Returns the list of supported exchange names</response>
    [HttpGet("exchanges/supported")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    public ActionResult<IEnumerable<string>> GetSupportedExchanges()
    {
        var supportedExchanges = exchangeService.GetSupportedExchanges();
        return Ok(supportedExchanges);
    }

    /// <summary>
    /// Gets an exchange by its name.
    /// </summary>
    /// <param name="name">The exchange name.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The exchange with the specified name.</returns>
    /// <response code="200">Returns the exchange</response>
    /// <response code="404">If the exchange is not found</response>
    [HttpGet("exchanges/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExchangeDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExchangeDto>> GetByName(
        [FromRoute] string name,
        CancellationToken cancellationToken = default)
    {
        var exchange = await exchangeService.GetByNameAsync(name, cancellationToken);
        return Ok(exchange);
    }

    /// <summary>
    /// Creates a new exchange.
    /// </summary>
    /// <param name="request">The exchange data.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created exchange.</returns>
    /// <response code="201">Returns the newly created exchange</response>
    /// <response code="400">If the exchange data is invalid</response>
    [HttpPost("exchanges")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ExchangeDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExchangeDto>> Create(
        [FromBody] CreateExchangeRequest request, 
        CancellationToken cancellationToken = default)
    {
        var createExchangeDto = mapper.Map<CreateExchangeDto>(request);
        var createdExchange = await exchangeService.AddAsync(createExchangeDto, cancellationToken);
        return CreatedAtAction(nameof(GetByName), new { name = createdExchange.Name }, createdExchange);
    }

    /// <summary>
    /// Deletes an exchange.
    /// </summary>
    /// <param name="name">The exchange name.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the exchange was successfully deleted</response>
    /// <response code="404">If the exchange is not found</response>
    [HttpDelete("exchanges/{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] string name, CancellationToken cancellationToken = default)
    {
        await exchangeService.DeleteAsync(name, cancellationToken);
        return NoContent();
    }
}
