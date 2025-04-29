using AutoMapper;
using InfoSymbolServer.Application.Abstractions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Application.Exceptions;
using InfoSymbolServer.Domain.Enums;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;

namespace InfoSymbolServer.Application.Services;

/// <inheritdoc/>
public class StatusService : IStatusService
{
    private readonly IStatusRepository _statusRepository;
    private readonly ISymbolRepository _symbolRepository;
    private readonly IExchangeRepository _exchangeRepository;
    private readonly IMapper _mapper;

    public StatusService(
        IStatusRepository statusRepository,
        ISymbolRepository symbolRepository,
        IExchangeRepository exchangeRepository,
        IMapper mapper)
    {
        _statusRepository = statusRepository;
        _symbolRepository = symbolRepository;
        _exchangeRepository = exchangeRepository;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<SymbolHistoryDto> GetSymbolHistoryAsync(
        string symbolName, 
        string exchangeName, 
        CancellationToken cancellationToken = default)
    {
        symbolName = Uri.UnescapeDataString(symbolName);
        
        var exchange = await _exchangeRepository.GetByNameAsync(exchangeName, cancellationToken)
            ?? throw new NotFoundException(nameof(Exchange), exchangeName);

        var symbol = await _symbolRepository.GetSingleByFilterAsync(
                s => s.SymbolName == symbolName && s.ExchangeId == exchange.Id,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Symbol), symbolName);

        var statuses = await _statusRepository.GetByFilterAsync(
            s => s.SymbolId == symbol.Id, cancellationToken: cancellationToken);

        return new SymbolHistoryDto
        {
            SymbolName = symbol.SymbolName,
            History = _mapper.Map<IEnumerable<StatusDto>>(statuses)
        };
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SymbolHistoryDto>> GetExchangeSymbolsHistoryAsync(
        string exchangeName, 
        int? pageNumber = null, 
        int? pageSize = null, 
        CancellationToken cancellationToken = default)
    {
        var exchange = await _exchangeRepository.GetByNameAsync(exchangeName, cancellationToken)
            ?? throw new NotFoundException(nameof(Exchange), exchangeName);

        var symbols = await _symbolRepository.GetByFilterAsync(
            s => s.ExchangeId == exchange.Id,
            pageNumber,
            pageSize,
            cancellationToken);

        var symbolIds = symbols.Select(s => s.Id).ToList();
        var allStatuses = await _statusRepository.GetByFilterAsync(
            s => symbolIds.Contains(s.SymbolId), cancellationToken: cancellationToken);
            
        var statusesBySymbolId = allStatuses.GroupBy(s => s.SymbolId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = symbols.Select(symbol => new SymbolHistoryDto
        {
            SymbolName = symbol.SymbolName,
            History = _mapper.Map<IEnumerable<StatusDto>>(
                statusesBySymbolId.TryGetValue(symbol.Id, out var statuses) ? statuses : [])
        }).ToList();

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SymbolHistoryDto>> GetExchangeActiveSymbolsHistoryAsync(
        string exchangeName, 
        int? pageNumber = null, 
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        var exchange = await _exchangeRepository.GetByNameAsync(exchangeName, cancellationToken)
            ?? throw new NotFoundException(nameof(Exchange), exchangeName);

        var symbols = await _symbolRepository.GetByFilterAsync(
            s => s.ExchangeId == exchange.Id && s.Status == SymbolStatus.Active,
            pageNumber,
            pageSize,
            cancellationToken);

        var symbolIds = symbols.Select(s => s.Id).ToList();
        var allStatuses = await _statusRepository.GetByFilterAsync(
            s => symbolIds.Contains(s.SymbolId), cancellationToken: cancellationToken);
            
        var statusesBySymbolId = allStatuses.GroupBy(s => s.SymbolId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = symbols.Select(symbol => new SymbolHistoryDto
        {
            SymbolName = symbol.SymbolName,
            History = _mapper.Map<IEnumerable<StatusDto>>(
                statusesBySymbolId.TryGetValue(symbol.Id, out var statuses) ? statuses : [])
        }).ToList();

        return result;
    }
}
