using FluentValidation;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Domain.Enums;

namespace InfoSymbolServer.Application.Validation.Validators;

/// <summary>
/// Validator for AddSymbolDto
/// </summary>
public class AddSymbolDtoValidator : AbstractValidator<AddSymbolDto>
{
    /// <summary>
    /// Configures validation rules for AddSymbolDto
    /// </summary>
    public AddSymbolDtoValidator()
    {
        RuleFor(x => x.ExchangeName)
            .NotEmpty().WithMessage("Exchange name is required")
            .MaximumLength(100).WithMessage("Exchange name must not exceed 100 characters");

        RuleFor(x => x.SymbolName)
            .NotEmpty().WithMessage("Symbol name is required")
            .MaximumLength(50).WithMessage("Symbol name must not exceed 50 characters");

        RuleFor(x => x.MarketType)
            .IsInEnum().WithMessage("Invalid market type");

        RuleFor(x => x.BaseAsset)
            .NotEmpty().WithMessage("Base asset is required")
            .MaximumLength(20).WithMessage("Base asset must not exceed 20 characters");

        RuleFor(x => x.QuoteAsset)
            .NotEmpty().WithMessage("Quote asset is required")
            .MaximumLength(20).WithMessage("Quote asset must not exceed 20 characters");

        RuleFor(x => x.PricePrecision)
            .GreaterThanOrEqualTo(0).WithMessage("Price precision must be non-negative");

        RuleFor(x => x.QuantityPrecision)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity precision must be non-negative");

        RuleFor(x => x.ContractType)
            .IsInEnum().WithMessage("Invalid contract type")
            .When(x => x.ContractType.HasValue);

        RuleFor(x => x.MarginAsset)
            .NotEmpty().WithMessage("Margin asset is required")
            .MaximumLength(20).WithMessage("Margin asset must not exceed 20 characters");

        RuleFor(x => x.MinQuantity)
            .GreaterThan(0).WithMessage("Minimum quantity must be greater than zero");

        RuleFor(x => x.MinNotional)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum notional must be greater than or equal to zero");

        RuleFor(x => x.MaxQuantity)
            .GreaterThan(0).WithMessage("Maximum quantity must be greater than zero")
            .GreaterThanOrEqualTo(x => x.MinQuantity)
                .WithMessage("Maximum quantity must be greater than or equal to minimum quantity");
    }
} 