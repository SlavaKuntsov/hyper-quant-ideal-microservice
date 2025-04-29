namespace InfoSymbolServer.Infrastructure.Abstractions;

/// <summary>
/// Service for sending emergency notifications about critical situations
/// </summary>
public interface IEmergencyNotificationService
{
    /// <summary>
    /// Sends notification about an error during exchange API communication
    /// </summary>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="errorMessage">The error message</param>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendExchangeApiErrorNotificationAsync(string exchangeName, string errorMessage, Exception? exception = null);
    
    /// <summary>
    /// Sends notification about a database access error
    /// </summary>
    /// <param name="operation">The database operation that failed</param>
    /// <param name="errorMessage">The error message</param>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendDatabaseErrorNotificationAsync(string operation, string errorMessage, Exception? exception = null);
    
    /// <summary>
    /// Sends notification about a critical system error
    /// </summary>
    /// <param name="component">The system component that failed</param>
    /// <param name="errorMessage">The error message</param>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendSystemErrorNotificationAsync(string component, string errorMessage, Exception? exception = null);
}
