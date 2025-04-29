using System.Text;
using InfoSymbolServer.Infrastructure.Abstractions;

namespace InfoSymbolServer.Infrastructure.Notifications.Formatters;

/// <summary>
/// Formats emergency notifications into HTML messages
/// </summary>
public class HtmlEmergencyNotificationMessageFormatter : IEmergencyNotificationMessageFormatter
{
    /// <inheritdoc />
    public string FormatExchangeApiErrorMessage(
        string exchangeName, string errorMessage, Exception? exception = null)
    {
        return FormatErrorMessage(
            "Exchange API Error",
            new Dictionary<string, string> { { "Exchange", exchangeName } },
            errorMessage,
            exception,
            "Please take immediate action to resolve this issue.");
    }

    /// <inheritdoc />
    public string FormatDatabaseErrorMessage(
        string operation, string errorMessage, Exception? exception = null)
    {
        return FormatErrorMessage(
            "Database Error",
            new Dictionary<string, string> { { "Operation", operation } },
            errorMessage, 
            exception,
            "Please take immediate action to resolve this database issue.");
    }

    /// <inheritdoc />
    public string FormatSystemErrorMessage(
        string component, string errorMessage, Exception? exception = null)
    {
        return FormatErrorMessage(
            "System Error",
            new Dictionary<string, string> { { "Component", component } },
            errorMessage,
            exception,
            "Please investigate the system error as soon as possible.");
    }
    
    /// <summary>
    /// Creates a formatted error message with consistent styling and content
    /// </summary>
    /// <param name="title">The error title</param>
    /// <param name="metadata">Key-value pairs of metadata to display</param>
    /// <param name="errorMessage">The main error message</param>
    /// <param name="exception">Optional exception details</param>
    /// <param name="actionMessage">Message suggesting action to take</param>
    /// <returns>Formatted HTML message</returns>
    private string FormatErrorMessage(
        string title, 
        Dictionary<string, string> metadata, 
        string errorMessage, 
        Exception? exception, 
        string actionMessage)
    {
        var body = new StringBuilder();
        
        body.AppendLine($"<h2 style='color: #cc0000;'>{title}</h2>");
        
        // Add all metadata
        foreach (var item in metadata)
        {
            body.AppendLine($"<p><strong>{item.Key}:</strong> {item.Value}</p>");
        }
        
        body.AppendLine($"<p><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>");
        body.AppendLine($"<p><strong>Error Message:</strong> {errorMessage}</p>");

        // Add exception details if any
        AppendExceptionDetails(body, exception);
        
        body.AppendLine($"<p>{actionMessage}</p>");

        return body.ToString();
    }
    
    private void AppendExceptionDetails(StringBuilder body, Exception? exception)
    {
        if (exception != null)
        {
            body.AppendLine("<h3>Exception Details:</h3>");
            body.AppendLine("<pre style='background-color: #f8f8f8; padding: 10px; border-radius: 5px;'>");
            body.AppendLine($"Type: {exception.GetType().FullName}");
            body.AppendLine($"Message: {exception.Message}");
            body.AppendLine($"Stack Trace: {exception.StackTrace}");
            
            if (exception.InnerException != null)
            {
                body.AppendLine($"Inner Exception: {exception.InnerException.Message}");
            }
            
            body.AppendLine("</pre>");
        }
    }
}
