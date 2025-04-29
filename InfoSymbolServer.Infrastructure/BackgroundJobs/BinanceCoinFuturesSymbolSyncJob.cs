using InfoSymbolServer.Domain.Enums;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;
using InfoSymbolServer.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;
using Quartz;
using UltimaBinanceConnector.Interfaces.Clients;
using UltimaBinanceConnector.Objects.Models.Futures;

namespace InfoSymbolServer.Infrastructure.BackgroundJobs
{
    [DisallowConcurrentExecution]
    public class BinanceCoinFuturesSymbolSyncJob : BaseBinanceSymbolSyncJob<BinanceCoinFuturesSymbolSyncJob>
    {
        public BinanceCoinFuturesSymbolSyncJob(
            ILogger<BinanceCoinFuturesSymbolSyncJob> logger,
            ISymbolRepository symbolRepository,
            IStatusRepository statusRepository,
            IExchangeRepository exchangeRepository,
            IUnitOfWork unitOfWork,
            IBinanceRestClient binanceRestClient,
            IEnumerable<INotificationService> notificationServices,
            IEmergencyNotificationService emergencyNotificationService)
            : base(logger, symbolRepository, statusRepository, exchangeRepository, unitOfWork, binanceRestClient, notificationServices, emergencyNotificationService)
        {
        }

        protected override string ExchangeName => "BinanceCoinFutures";
        protected override MarketType MarketType => MarketType.CoinFutures;

        protected override async Task<IEnumerable<object>> FetchSymbolsAsync()
        {
            var response = await BinanceRestClient.CoinFuturesApi.ExchangeData.GetExchangeInfoAsync();
            return response.Data.Symbols;
        }

        protected override bool MapSymbol(Symbol symbol, object binanceSymbol)
        {
            return BinanceSymbolMapper.MapCoinFuturesSymbol(symbol, (BinanceFuturesCoinSymbol)binanceSymbol);
        }

        protected override string GetSymbolName(object binanceSymbol)
        {
            return ((BinanceFuturesCoinSymbol)binanceSymbol).Name;
        }
    }
}
