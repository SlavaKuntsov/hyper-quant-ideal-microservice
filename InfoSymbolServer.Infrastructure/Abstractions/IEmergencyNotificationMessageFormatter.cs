namespace InfoSymbolServer.Infrastructure.Abstractions;

/// <summary>
/// Interface for creating formatted emergency messages
/// </summary>
public interface IEmergencyNotificationMessageFormatter
{
    /// <summary>
    /// Formats a message for exchange API errors
    /// </summary>
    /// <param name="exchangeName">The name of the exchange</param>
    /// <param name="errorMessage">The error message</param>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>A tuple containing the subject and body of the email</returns>
    string FormatExchangeApiErrorMessage(
        string exchangeName, string errorMessage, Exception? exception = null);
    
    /// <summary>
    /// Formats a message for database errors
    /// </summary>
    /// <param name="operation">The database operation that failed</param>
    /// <param name="errorMessage">The error message</param>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>A tuple containing the subject and body of the email</returns>
    string FormatDatabaseErrorMessage(
        string operation, string errorMessage, Exception? exception = null);
    
    /// <summary>
    /// Formats a message for system errors
    /// </summary>
    /// <param name="component">The system component that failed</param>
    /// <param name="errorMessage">The error message</param>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>A tuple containing the subject and body of the email</returns>
    string FormatSystemErrorMessage(
        string component, string errorMessage, Exception? exception = null);
}
