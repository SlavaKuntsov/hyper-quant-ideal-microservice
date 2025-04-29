using FluentValidation;
using ValidationException = InfoSymbolServer.Application.Exceptions.ValidationException;

namespace InfoSymbolServer.Application.Validation;

/// <summary>
/// Pipeline for auto-validating objects
/// </summary>
public class ValidationPipeline(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Validates an object
    /// </summary>
    /// <typeparam name="TDto">Type of the DTO to validate</typeparam>
    /// <param name="dto">The DTO to validate</param>
    /// <exception cref="ValidationException">Thrown when validation fails</exception>
    public void Validate<TDto>(TDto dto) where TDto : class
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(typeof(TDto));

        if (serviceProvider.GetService(validatorType) is IValidator validator)
        {
            var context = new ValidationContext<TDto>(dto);
            var result = validator.Validate(context);

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());
                
                throw new ValidationException(errors);
            }
        }
    }

    /// <summary>
    /// Validates an object asynchronously
    /// </summary>
    /// <typeparam name="TDto">Type of the DTO to validate</typeparam>
    /// <param name="dto">The DTO to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ValidationException">Thrown when validation fails</exception>
    public async Task ValidateAsync<TDto>(TDto dto, CancellationToken cancellationToken = default)
        where TDto : class
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(typeof(TDto));
        if (serviceProvider.GetService(validatorType) is IValidator validator)
        {
            var context = new ValidationContext<TDto>(dto);
            var result = await validator.ValidateAsync(context, cancellationToken);

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());
                
                throw new ValidationException(errors);
            }
        }
    }
}
