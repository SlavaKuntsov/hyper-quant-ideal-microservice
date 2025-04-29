using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InfoSymbolServer.Presentation.Filters;

/// <summary>
/// Action filter to validate pagination parameters
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ValidatePaginationAttribute : ActionFilterAttribute
{
    private const int MinPageNumber = 1;
    private const int MinPageSize = 1;
    
    /// <summary>
    /// Validates pagination parameters before the action executes
    /// </summary>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Tries to retrieve parameter page, and validates it is parameter exists.
        if (context.ActionArguments.TryGetValue("page", out var pageObj) && 
            pageObj is int page)
        {
            if (page < MinPageNumber)
            {
                context.ModelState.AddModelError(
                    "page", $"Page must be greater than or equal to {MinPageNumber}");
            }
        }

        // Tries to retrieve parameter pageNumber, and validates it is parameter exists.
        if (context.ActionArguments.TryGetValue("pageSize", out var pageSizeObj) && 
            pageSizeObj is int pageSize)
        {
            if (pageSize < MinPageSize)
            {
                context.ModelState.AddModelError(
                    "pageSize", $"PageSize must be greater than or equal to {MinPageSize}");
            }
        }

        // If any of page and pageNumber parameters is invalid, returns BadRequest response.
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}
