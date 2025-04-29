using FluentValidation;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Domain.Enums;

namespace InfoSymbolServer.Application.Validation.Validators;

/// <summary>
/// Validator for CreateExchangeDto
/// </summary>
public class CreateExchangeDtoValidator : AbstractValidator<CreateExchangeDto>
{
    /// <summary>
    /// Configures validation rules for CreateExchangeDto
    /// </summary>
    public CreateExchangeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Exchange name is required")
            .MaximumLength(100).WithMessage("Exchange name must not exceed 100 characters");
    }
}
