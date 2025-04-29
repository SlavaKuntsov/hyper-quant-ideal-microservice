using InfoSymbolServer.Domain.Enums;
using InfoSymbolServer.Domain.Models;
using InfoSymbolServer.Domain.Repositories;
using InfoSymbolServer.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;
using Quartz;
using UltimaBinanceConnector.Interfaces.Clients;
using UltimaBinanceConnector.Objects.Models.Spot;

namespace InfoSymbolServer.Infrastructure.BackgroundJobs
{
    [DisallowConcurrentExecution]
    public class BinanceSpotSymbolSyncJob : BaseBinanceSymbolSyncJob<BinanceSpotSymbolSyncJob>
    {
        public BinanceSpotSymbolSyncJob(
            ILogger<BinanceSpotSymbolSyncJob> logger,
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

        protected override string ExchangeName => "BinanceSpot";
        protected override MarketType MarketType => MarketType.Spot;

        protected override async Task<IEnumerable<object>> FetchSymbolsAsync()
        {
            var response = await BinanceRestClient.SpotApi.ExchangeData.GetExchangeInfoAsync();
            return response.Data.Symbols;
        }

        protected override bool MapSymbol(Symbol symbol, object binanceSymbol)
        {
            return BinanceSymbolMapper.MapSpotSymbol(symbol, (BinanceSymbol)binanceSymbol);
        }

        protected override string GetSymbolName(object binanceSymbol)
        {
            return ((BinanceSymbol)binanceSymbol).Name;
        }
    }
}
