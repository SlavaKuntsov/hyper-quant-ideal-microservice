using System.Linq.Expressions;
using InfoSymbolServer.Domain.Models;

namespace InfoSymbolServer.Domain.Repositories;

/// <summary>
/// Repository to operate with status records
/// </summary>
public interface IStatusRepository
{
    /// <summary>
    /// Gets all status records
    /// </summary>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of all status records</returns>
    Task<IEnumerable<Status>> GetAllAsync(
        int? pageNumber = null, 
        int? pageSize = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets status records that match the specified filter
    /// </summary>
    /// <param name="filter">Expression to filter status records</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of filtered status records</returns>
    Task<IEnumerable<Status>> GetByFilterAsync(
        Expression<Func<Status, bool>> filter, 
        int? pageNumber = null, 
        int? pageSize = null, 
        CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Gets a single status record that matches the specified filter
    /// </summary>
    /// <param name="filter">Expression to filter status records</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The status record if found, otherwise null</returns>
    Task<Status?> GetSingleByFilterAsync(
        Expression<Func<Status, bool>> filter, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new status record
    /// </summary>
    /// <param name="status">The status to add</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The added status with generated ID</returns>
    Task AddAsync(Status status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a range of new status records
    /// </summary>
    /// <param name="statuses">Status records to add</param>
    /// <param name="cancellationToken"></param>
    Task AddRangeAsync(IEnumerable<Status> statuses, CancellationToken cancellationToken = default);
} 