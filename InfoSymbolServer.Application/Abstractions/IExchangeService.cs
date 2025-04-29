using System.Linq.Expressions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Domain.Models;

namespace InfoSymbolServer.Application.Abstractions;

/// <summary>
/// Service for managing exchanges
/// </summary>
public interface IExchangeService
{
    /// <summary>
    /// Gets all exchanges
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of all exchanges</returns>
    Task<IEnumerable<ExchangeDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an exchange by its name
    /// </summary>
    /// <param name="name">The exchange name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The exchange if found, otherwise null</returns>
    Task<ExchangeDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new exchange
    /// </summary>
    /// <param name="exchangeDto">The exchange data to add</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The added exchange with generated ID</returns>
    Task<ExchangeDto> AddAsync(CreateExchangeDto exchangeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an exchange
    /// </summary>
    /// <param name="name">The name of the exchange to delete</param>
    /// <param name="cancellationToken"></param>
    Task DeleteAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all supported exchanges
    /// </summary>
    /// <returns>Collection of supported exchange names</returns>
    IEnumerable<string> GetSupportedExchanges();
}
