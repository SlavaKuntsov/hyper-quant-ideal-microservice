using InfoSymbolServer.Domain.Enums;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;
using InfoSymbolServer.Infrastructure.Abstractions;
using InfoSymbolServer.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Quartz;
using UltimaBinanceConnector.Interfaces.Clients;

namespace InfoSymbolServer.Infrastructure.BackgroundJobs
{
    public abstract class BaseBinanceSymbolSyncJob<TLogger>(
        ILogger<TLogger> logger,
        ISymbolRepository symbolRepository,
        IStatusRepository statusRepository,
        IExchangeRepository exchangeRepository,
        IUnitOfWork unitOfWork,
        IBinanceRestClient binanceRestClient,
        IEnumerable<INotificationService> notificationServices,
        IEmergencyNotificationService emergencyNotificationService) : IJob where TLogger : class
    {
        protected readonly IBinanceRestClient BinanceRestClient = binanceRestClient;
        protected readonly IEnumerable<INotificationService> NotificationServices = notificationServices;
        protected abstract string ExchangeName { get; }
        protected abstract MarketType MarketType { get; }
        
        protected abstract Task<IEnumerable<object>> FetchSymbolsAsync();
        protected abstract bool MapSymbol(Symbol symbol, object binanceSymbol);
        protected abstract string GetSymbolName(object binanceSymbol);

        /// <summary>
        /// Executes the job.
        /// </summary>
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation(
                "Starting {Exchange} symbols synchronization at {Time}", ExchangeName, DateTime.UtcNow);

            try
            {
                // Tries to fetch exchange from database, if not exists - job will not be executed.
                var exchange = await exchangeRepository.GetByNameAsync(ExchangeName);
                if (exchange == null)
                {
                    logger.LogWarning("{Exchange} exchange not found in the database.", ExchangeName);
                    return;
                }

                // Fetches symbols for this exchange from the database.
                var existingSymbols = await symbolRepository
                    .GetByFilterAsync(s => s.ExchangeId == exchange.Id && s.MarketType == MarketType);
                var existingSymbolsDict = existingSymbols
                    .ToDictionary(s => s.SymbolName);

                var updatedSymbols = new List<Symbol>();
                var newSymbols = new List<Symbol>();
                var removedSymbols = new List<Symbol>();
                var statusChanges = new List<Status>();
                var processedSymbols = new HashSet<string>();

                // Process this market type symbols
                await ProcessSymbolsAsync(
                    existingSymbolsDict,
                    updatedSymbols,
                    newSymbols,
                    removedSymbols,
                    statusChanges,
                    processedSymbols,
                    exchange.Id);
                
                // Saves changes to the database if any changes were made
                await SaveSymbolsAsync(
                    updatedSymbols,
                    newSymbols,
                    removedSymbols,
                    statusChanges);

                // Sends notifications for changes in symbols if any changes were made
                if (updatedSymbols.Count > 0 || newSymbols.Count > 0 || removedSymbols.Count > 0)
                {
                    foreach (var notificationService in NotificationServices)
                    {
                        await notificationService.SendCombinedSymbolChangesNotificationAsync(
                            newSymbols,
                            updatedSymbols,
                            removedSymbols,
                            ExchangeName,
                            MarketType.ToString());
                    }
                }

                logger.LogInformation(
                    "{Exchange} symbol synchronization finished: {UpdatedCount} symbols updated, " +
                    "{NewCount} symbols added, {DelistedCount} symbols removed, {StatusChanges} status changes recorded",
                    ExchangeName, updatedSymbols.Count, newSymbols.Count, removedSymbols.Count, statusChanges.Count);
            }
            catch (ExchangeApiException ex)
            {
                logger.LogError(ex, "Exchange API error during {Exchange} symbol synchronization: {Message}", 
                    ExchangeName, ex.Message);
                await emergencyNotificationService.SendExchangeApiErrorNotificationAsync(
                    ex.ExchangeName, $"Error during {ex.ExchangeName} {ex.MarketType} symbols synchronization", ex);
            }
            catch (DatabaseException ex)
            {
                logger.LogError(ex, "Database error during {Exchange} symbol synchronization: {Message}", 
                    ExchangeName, ex.Message);
                await emergencyNotificationService.SendDatabaseErrorNotificationAsync(
                    ex.ContextName, $"Error updating symbols: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during {Exchange} symbol synchronization: {Message}", 
                    ExchangeName, ex.Message);
                await emergencyNotificationService.SendSystemErrorNotificationAsync(
                    "SymbolSyncJob", $"Unexpected error during {ExchangeName} {MarketType} symbols synchronization", ex);
            }
        }

        /// <summary>
        /// Process symbols for concrete market type.
        /// </summary>
        private async Task ProcessSymbolsAsync(
            Dictionary<string, Symbol> existingSymbolsDict,
            List<Symbol> updatedSymbols,
            List<Symbol> newSymbols,
            List<Symbol> removedSymbols,
            List<Status> statusChanges,
            HashSet<string> processedSymbols,
            Guid exchangeId)
        {
            try
            {
                // Fetches symbols from Binance API
                var binanceSymbols = await FetchSymbolsAsync();
                    
                foreach (var binanceSymbol in binanceSymbols)
                {
                    var symbolName = GetSymbolName(binanceSymbol);
                    processedSymbols.Add(symbolName);
                
                    if (existingSymbolsDict.TryGetValue(symbolName, out var symbol) &&
                        symbol.Status != SymbolStatus.RemovedByAdmin)
                    {
                        TryUpdateSymbol(updatedSymbols, statusChanges, binanceSymbol, symbol);
                    }
                    else
                    {
                        // Creates new symbol entry in the database
                        CreateSymbol(newSymbols, statusChanges, exchangeId, binanceSymbol);
                    }
                }

                // Tries to set symbols as delisted if they don't exist in the Binance API response.
                TryUpdateDelistedSymbols(existingSymbolsDict, statusChanges, processedSymbols, removedSymbols);
            }
            catch (Exception ex)
            {
                throw new ExchangeApiException(
                    ExchangeName, 
                    MarketType.ToString(), 
                    $"Error during {ExchangeName} {MarketType} symbol synchronization", 
                    ex);
            }
        }

        /// <summary>
        /// Tries to update symbol if it exists in the database.
        /// </summary>
        private void TryUpdateSymbol(
            List<Symbol> updatedSymbols, List<Status> statusChanges, object binanceSymbol, Symbol symbol)
        {
            // If ShouldSynchronize set to true, and symbol was not deleted by admin, tries to update it
            if (symbol.Status != SymbolStatus.RemovedByAdmin)
            {
                // Save current status before mapping & update symbol
                var oldStatus = symbol.Status;
                var wasUpdated = MapSymbol(symbol, binanceSymbol);

                // If status changed, add status change record
                if (oldStatus != symbol.Status)
                {
                    statusChanges.Add(new Status
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        SymbolId = symbol.Id,
                        SymbolStatus = symbol.Status
                    });
                }

                // Only add to updated list if something was actually updated
                if (wasUpdated)
                {
                    symbol.UpdatedAt = DateTime.UtcNow;
                    updatedSymbols.Add(symbol);
                }
            }
        }

        /// <summary>
        /// Creates new symbol entry in the database.
        /// </summary>
        private void CreateSymbol(
            List<Symbol> newSymbols, List<Status> statusChanges, Guid exchangeId, object binanceSymbol)
        {
            // Otherwise creates new entry
            var symbol = new Symbol
            {
                Id = Guid.NewGuid(),
                ExchangeId = exchangeId,
                UpdatedAt = DateTime.UtcNow
            };
            MapSymbol(symbol, binanceSymbol);

            // Add status change record for the new symbol
            statusChanges.Add(new Status
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                SymbolId = symbol.Id,
                SymbolStatus = symbol.Status
            });

            newSymbols.Add(symbol);
        }

        /// <summary>
        /// Updates the status of delisted symbols to Delisted.
        /// </summary>
        private void TryUpdateDelistedSymbols(
            Dictionary<string, Symbol> existingSymbolsDict,
            List<Status> statusChanges,
            HashSet<string> processedSymbols,
            List<Symbol> removedSymbols)
        {
            foreach (var symbolName in existingSymbolsDict.Keys)
            {
                if (!processedSymbols.Contains(symbolName))
                {
                    var symbol = existingSymbolsDict[symbolName];
                    if (symbol.Status != SymbolStatus.Delisted && symbol.Status != SymbolStatus.RemovedByAdmin)
                    {
                        // Record status change before updating
                        symbol.Status = SymbolStatus.Delisted;
                        symbol.UpdatedAt = DateTime.UtcNow;

                        // Add status change record
                        statusChanges.Add(new Status
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            SymbolId = symbol.Id,
                            SymbolStatus = SymbolStatus.Delisted
                        });

                        removedSymbols.Add(symbol);
                    }
                }
            }
        }

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        private async Task SaveSymbolsAsync(
            List<Symbol> updatedSymbols,
            List<Symbol> newSymbols,
            List<Symbol> removedSymbols,
            List<Status> statusChanges)
        {
            if (updatedSymbols.Count == 0 && newSymbols.Count == 0 && removedSymbols.Count == 0)
            {
                return;
            }

            try
            {
                await symbolRepository.UpdateRangeAsync(updatedSymbols);
                await symbolRepository.AddRangeAsync(newSymbols);
                await symbolRepository.UpdateRangeAsync(removedSymbols);
                await statusRepository.AddRangeAsync(statusChanges);

                await unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException(
                    "SymbolSyncJob", $"Error updating symbols for {ExchangeName} {MarketType}", ex);
            }
        }
    }
}
