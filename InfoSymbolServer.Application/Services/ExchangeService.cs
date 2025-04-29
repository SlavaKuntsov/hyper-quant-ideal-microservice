using AutoMapper;
using InfoSymbolServer.Application.Abstractions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Application.Exceptions;
using InfoSymbolServer.Application.Validation;
using InfoSymbolServer.Domain.Constants;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;

namespace InfoSymbolServer.Application.Services;

/// <inheritdoc/>
public class ExchangeService : IExchangeService
{
    private readonly IExchangeRepository _exchangeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ValidationPipeline _validationPipeline;

    public ExchangeService(
        IExchangeRepository exchangeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ValidationPipeline validationPipeline)
    {
        _exchangeRepository = exchangeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationPipeline = validationPipeline;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ExchangeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var exchanges = await _exchangeRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ExchangeDto>>(exchanges);
    }

    /// <inheritdoc />
    public async Task<ExchangeDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var exchange = await _exchangeRepository.GetByNameAsync(name, cancellationToken);
        if (exchange == null)
        {
            throw new NotFoundException(nameof(Exchange), name);
        }

        return _mapper.Map<ExchangeDto>(exchange);
    }

    /// <inheritdoc />
    public async Task<ExchangeDto> AddAsync(
        CreateExchangeDto exchangeDto,
        CancellationToken cancellationToken = default)
    {
        await _validationPipeline.ValidateAsync(exchangeDto, cancellationToken);
        var exchange = _mapper.Map<Exchange>(exchangeDto);

        var existingExchange = await _exchangeRepository.GetByNameAsync(exchange.Name, cancellationToken);
        if (existingExchange != null)
        {
            throw new ValidationException(nameof(Exchange), exchange.Name);
        }

        if (!GetSupportedExchanges().Contains(exchange.Name))
        {
            throw new ValidationException(nameof(Exchange), $"Exchange {exchange.Name} is not supported.");
        }

        await _exchangeRepository.AddAsync(exchange, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<ExchangeDto>(exchange);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string name, CancellationToken cancellationToken = default)
    {
        var exchange = await _exchangeRepository.GetByNameAsync(name, cancellationToken)
            ?? throw new NotFoundException(nameof(Exchange), name);

        await _exchangeRepository.DeleteAsync(exchange, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IEnumerable<string> GetSupportedExchanges()
    {
        return
        [
            SupportedExchanges.BinanceSpot,
            SupportedExchanges.BinanceUsdtFutures,
            SupportedExchanges.BinanceCoinFutures
        ];
    }
}
