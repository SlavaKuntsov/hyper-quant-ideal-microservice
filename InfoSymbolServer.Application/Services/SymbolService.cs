using AutoMapper;
using InfoSymbolServer.Application.Abstractions;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Application.Exceptions;
using InfoSymbolServer.Application.Validation;
using InfoSymbolServer.Domain.Enums;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;

namespace InfoSymbolServer.Application.Services;

/// <inheritdoc/>
public class SymbolService(
    ISymbolRepository symbolRepository,
    IStatusRepository statusRepository,
    IExchangeRepository exchangeRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ValidationPipeline validationPipeline) : ISymbolService
{
    /// <inheritdoc />
    public async Task<IEnumerable<SymbolDto>> GetForExchangeAsync(
        string exchangeName, 
        int? pageNumber = null,
        int? pageSize = null, 
        CancellationToken cancellationToken = default)
    {
        var exchange = await exchangeRepository
            .GetByNameAsync(exchangeName, cancellationToken) ??
            throw new NotFoundException(nameof(Exchange), exchangeName);
        
        var symbols = await symbolRepository
            .GetByFilterAsync(e => e.ExchangeId == exchange.Id, pageNumber, pageSize, cancellationToken);

        return mapper.Map<IEnumerable<SymbolDto>>(symbols);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SymbolDto>> GetActiveForExchangeAsync(
        string exchangeName, 
        int? pageNumber = null,
        int? pageSize = null, 
        CancellationToken cancellationToken = default)
    {
        var exchange = await exchangeRepository.GetByNameAsync(exchangeName, cancellationToken)
            ?? throw new NotFoundException(nameof(Exchange), exchangeName);
            
        var symbols = await symbolRepository.GetByFilterAsync(
            s => s.ExchangeId == exchange.Id && s.Status == SymbolStatus.Active,
            pageNumber,
            pageSize,
            cancellationToken);
            
        return mapper.Map<IEnumerable<SymbolDto>>(symbols);
    }

    /// <inheritdoc />
    public async Task<SymbolDto?> GetForExchangeByNameAsync(
        string symbolName, 
        string exchangeName, 
        CancellationToken cancellationToken = default)
    {
        // URL decode the symbol name if it's encoded. Used to work correcly with symbols like BTC/USDT (with /)
        symbolName = Uri.UnescapeDataString(symbolName);
        var exchange = await exchangeRepository.GetByNameAsync(exchangeName, cancellationToken)
            ?? throw new NotFoundException(nameof(Exchange), exchangeName);
            
        var symbol = await symbolRepository.GetSingleByFilterAsync(
            s => s.SymbolName == symbolName && s.ExchangeId == exchange.Id,
            cancellationToken);

        if (symbol == null)
        {
            throw new NotFoundException(nameof(Symbol), symbolName);
        }
        
        return mapper.Map<SymbolDto>(symbol);
    }
    
    /// <inheritdoc />
    public async Task<SymbolDto> AddAsync(
        AddSymbolDto addSymbolDto, 
        CancellationToken cancellationToken = default)
    {
        // Validate the DTO using the validation pipeline
        await validationPipeline.ValidateAsync(addSymbolDto, cancellationToken);
        
        // Try to get exchange and symbol
        var exchange = await exchangeRepository.GetByNameAsync(addSymbolDto.ExchangeName, cancellationToken)
            ?? throw new NotFoundException(nameof(Exchange), addSymbolDto.ExchangeName);
            
        var existingSymbol = await symbolRepository.GetSingleByFilterAsync(
            s => s.SymbolName == addSymbolDto.SymbolName && s.ExchangeId == exchange.Id, 
            cancellationToken);
        
        // If symbol exists in any state, throw a validation exception
        if (existingSymbol != null)
        {
            throw new ValidationException(nameof(Symbol), "Symbol already exists");
        }
        
        // Create new symbol using mapper
        var newSymbol = mapper.Map<Symbol>(addSymbolDto);
        newSymbol.Id = Guid.NewGuid();
        newSymbol.ExchangeId = exchange.Id;
        newSymbol.Status = SymbolStatus.AddedByAdmin;
        newSymbol.UpdatedAt = DateTime.UtcNow;
        
        await symbolRepository.AddAsync(newSymbol, cancellationToken);
        
        // Add status record for the new symbol
        await statusRepository.AddAsync(new Status
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            SymbolId = newSymbol.Id,
            SymbolStatus = SymbolStatus.AddedByAdmin
        }, cancellationToken);
        
        await unitOfWork.SaveAsync(cancellationToken);
        return mapper.Map<SymbolDto>(newSymbol);
    }
    
    /// <inheritdoc />
    public async Task<SymbolDto> DeleteAsync(
        string symbolName, 
        string exchangeName, 
        CancellationToken cancellationToken = default)
    {
        // Try to get exchange and symbol
        var exchange = await exchangeRepository.GetByNameAsync(exchangeName, cancellationToken)
            ?? throw new NotFoundException(nameof(Exchange), exchangeName);
            
        var symbol = await symbolRepository
            .GetSingleByFilterAsync(s => s.SymbolName == symbolName && s.ExchangeId == exchange.Id, cancellationToken)
            ?? throw new NotFoundException($"Symbol {symbolName} on exchange {exchangeName}");
        
        // If symbol is already removed by admin, throw validation exception
        if (symbol.Status == SymbolStatus.RemovedByAdmin)
        {
            throw new ValidationException(nameof(Symbol), "Symbol is already removed by admin");
        }

        // Set status to RemovedByAdmin and update timestamp
        symbol.Status = SymbolStatus.RemovedByAdmin;
        symbol.UpdatedAt = DateTime.UtcNow;
        
        // Update symbol and add status change record
        await symbolRepository.UpdateAsync(symbol, cancellationToken);
        await statusRepository.AddAsync(new Status
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            SymbolId = symbol.Id,
            SymbolStatus = SymbolStatus.RemovedByAdmin
        }, cancellationToken);
        
        await unitOfWork.SaveAsync(cancellationToken);
        return mapper.Map<SymbolDto>(symbol);
    }

    /// <inheritdoc />
    public async Task<SymbolDto> RevokeDeleteAsync(
        string symbolName, 
        string exchangeName, 
        CancellationToken cancellationToken = default)
    {
        // Try to get exchange and symbol
        var exchange = await exchangeRepository.GetByNameAsync(exchangeName, cancellationToken)
            ?? throw new NotFoundException(nameof(Exchange), exchangeName);
            
        var symbol = await symbolRepository
            .GetSingleByFilterAsync(s => s.SymbolName == symbolName && s.ExchangeId == exchange.Id, cancellationToken)
            ?? throw new NotFoundException($"Symbol {symbolName} on exchange {exchangeName}");
        
        // If symbol is not in RemovedByAdmin status, throw validation exception
        if (symbol.Status != SymbolStatus.RemovedByAdmin)
        {
            throw new ValidationException(nameof(Symbol), "Only symbols with RemovedByAdmin status can be revoked");
        }

        // Set status to AddedByAdmin and update timestamp
        symbol.Status = SymbolStatus.AddedByAdmin;
        symbol.UpdatedAt = DateTime.UtcNow;
        
        // Update symbol and add status change record
        await symbolRepository.UpdateAsync(symbol, cancellationToken);
        await statusRepository.AddAsync(new Status
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            SymbolId = symbol.Id,
            SymbolStatus = SymbolStatus.AddedByAdmin
        }, cancellationToken);
        
        await unitOfWork.SaveAsync(cancellationToken);
        return mapper.Map<SymbolDto>(symbol);
    }
}
