using System.Globalization;
using InfoSymbolServer.Domain.Enums;
using InfoSymbolServer.Domain.Models;
using UltimaBinanceConnector.Objects.Models.Futures;
using UltimaBinanceConnector.Objects.Models.Spot;
using ContractType = InfoSymbolServer.Domain.Enums.ContractType;
using SymbolStatus = InfoSymbolServer.Domain.Enums.SymbolStatus;
using BinanceSymbolStatus = UltimaBinanceConnector.Enums.SymbolStatus;
using BinanceContractType = UltimaBinanceConnector.Enums.ContractType;

namespace InfoSymbolServer.Infrastructure.BackgroundJobs;

/// <summary>
/// Maps data between Binance Symbol model and internal Symbol model
/// </summary>
public static class BinanceSymbolMapper
{
    /// <summary>
    /// Maps a Binance spot symbol to the unified Symbol model.
    /// </summary>
    /// <returns>True, if any property was updated, instead false.</returns>
    public static bool MapSpotSymbol(Symbol target, BinanceSymbol source)
    {
        var updated = false;

        if (target.SymbolName != source.Name)
        {
            target.SymbolName = source.Name;
            updated = true;
        }
        if (target.MarketType != MarketType.Spot)
        {
            target.MarketType = MarketType.Spot;
            updated = true;
        }
        if (target.BaseAsset != source.BaseAsset)
        {
            target.BaseAsset = source.BaseAsset;
            updated = true;
        }
        if (target.QuoteAsset != source.QuoteAsset)
        {
            target.QuoteAsset = source.QuoteAsset;
            updated = true;
        }
        var mappedStatus = MapSymbolStatus(source.Status);
        if (target.Status != mappedStatus)
        {
            target.Status = mappedStatus;
            updated = true;
        }
        if (target.ContractType != ContractType.Spot)
        {
            target.ContractType = ContractType.Spot;
            updated = true;
        }
        if (target.DeliveryDate != null)
        {
            target.DeliveryDate = null;
            updated = true;
        }
        if (target.MarginAsset != null)
        {
            target.MarginAsset = null;
            updated = true;
        }

        var priceFilter = source.PriceFilter;
        var lotSizeFilter = source.LotSizeFilter;
        var notionalFilter = source.NotionalFilter;

        var pricePrecision = priceFilter != null ? CalculatePrecision(priceFilter.TickSize) : 0;
        var quantityPrecision = lotSizeFilter != null ? CalculatePrecision(lotSizeFilter.StepSize) : 0;

        if (target.PricePrecision != pricePrecision)
        {
            target.PricePrecision = pricePrecision;
            updated = true;
        }
        if (target.QuantityPrecision != quantityPrecision)
        {
            target.QuantityPrecision = quantityPrecision;
            updated = true;
        }
        if (lotSizeFilter != null)
        {
            if (target.MinQuantity != lotSizeFilter.MinQuantity)
            {
                target.MinQuantity = lotSizeFilter.MinQuantity;
                updated = true;
            }
            if (target.MaxQuantity != lotSizeFilter.MaxQuantity)
            {
                target.MaxQuantity = lotSizeFilter.MaxQuantity;
                updated = true;
            }
        }
        if (notionalFilter != null && target.MinNotional != notionalFilter.MinNotional)
        {
            target.MinNotional = notionalFilter.MinNotional;
            updated = true;
        }

        return updated;
    }

    /// <summary>
    /// Maps a Binance USDT futures symbol to the unified Symbol model.
    /// </summary>
    /// <returns>True, if any property was updated, instead false.</returns>
    public static bool MapUsdtFuturesSymbol(Symbol target, BinanceFuturesUsdtSymbol source)
    {
        var updated = false;

        if (target.SymbolName != source.Name)
        {
            target.SymbolName = source.Name;
            updated = true;
        }
        if (target.MarketType != MarketType.UsdtFutures)
        {
            target.MarketType = MarketType.UsdtFutures;
            updated = true;
        }
        if (target.BaseAsset != source.BaseAsset)
        {
            target.BaseAsset = source.BaseAsset;
            updated = true;
        }
        if (target.QuoteAsset != source.QuoteAsset)
        {
            target.QuoteAsset = source.QuoteAsset;
            updated = true;
        }
        var mappedStatus = MapSymbolStatus(source.Status);
        if (target.Status != mappedStatus)
        {
            target.Status = mappedStatus;
            updated = true;
        }
        var mappedContractType = MapContractType(source.ContractType);
        if (target.ContractType != mappedContractType)
        {
            target.ContractType = mappedContractType;
            updated = true;
        }
        if (target.DeliveryDate != source.DeliveryDate)
        {
            target.DeliveryDate = source.DeliveryDate;
            updated = true;
        }
        if (target.MarginAsset != source.MarginAsset)
        {
            target.MarginAsset = source.MarginAsset;
            updated = true;
        }
        if (target.PricePrecision != source.PricePrecision)
        {
            target.PricePrecision = source.PricePrecision;
            updated = true;
        }
        if (target.QuantityPrecision != source.QuantityPrecision)
        {
            target.QuantityPrecision = source.QuantityPrecision;
            updated = true;
        }

        // Filters
        var lotSizeFilter = source.LotSizeFilter;
        var minNotionalFilter = source.MinNotionalFilter;

        if (lotSizeFilter != null)
        {
            if (target.MinQuantity != lotSizeFilter.MinQuantity)
            {
                target.MinQuantity = lotSizeFilter.MinQuantity;
                updated = true;
            }
            if (target.MaxQuantity != lotSizeFilter.MaxQuantity)
            {
                target.MaxQuantity = lotSizeFilter.MaxQuantity;
                updated = true;
            }
        }
        if (minNotionalFilter != null && target.MinNotional != minNotionalFilter.MinNotional)
        {
            target.MinNotional = minNotionalFilter.MinNotional;
            updated = true;
        }

        return updated;
    }

    /// <summary>
    /// Maps a Binance coin futures symbol to the unified Symbol model.
    /// </summary>
    /// <returns>True, if any property was updated, instead false.</returns>
    public static bool MapCoinFuturesSymbol(Symbol target, BinanceFuturesCoinSymbol source)
    {
        var updated = false;

        if (target.SymbolName != source.Name)
        {
            target.SymbolName = source.Name;
            updated = true;
        }
        if (target.MarketType != MarketType.CoinFutures)
        {
            target.MarketType = MarketType.CoinFutures;
            updated = true;
        }
        if (target.BaseAsset != source.BaseAsset)
        {
            target.BaseAsset = source.BaseAsset;
            updated = true;
        }
        if (target.QuoteAsset != source.QuoteAsset)
        {
            target.QuoteAsset = source.QuoteAsset;
            updated = true;
        }
        var mappedStatus = MapSymbolStatus(source.Status);
        if (target.Status != mappedStatus)
        {
            target.Status = mappedStatus;
            updated = true;
        }
        var mappedContractType = MapContractType(source.ContractType);
        if (target.ContractType != mappedContractType)
        {
            target.ContractType = mappedContractType;
            updated = true;
        }
        if (target.DeliveryDate != source.DeliveryDate)
        {
            target.DeliveryDate = source.DeliveryDate;
            updated = true;
        }
        if (target.MarginAsset != source.MarginAsset)
        {
            target.MarginAsset = source.MarginAsset;
            updated = true;
        }
        if (target.PricePrecision != source.PricePrecision)
        {
            target.PricePrecision = source.PricePrecision;
            updated = true;
        }
        if (target.QuantityPrecision != source.QuantityPrecision)
        {
            target.QuantityPrecision = source.QuantityPrecision;
            updated = true;
        }

        // Filters
        var lotSizeFilter = source.LotSizeFilter;
        var minNotionalFilter = source.MinNotionalFilter;

        if (lotSizeFilter != null)
        {
            if (target.MinQuantity != lotSizeFilter.MinQuantity)
            {
                target.MinQuantity = lotSizeFilter.MinQuantity;
                updated = true;
            }
            if (target.MaxQuantity != lotSizeFilter.MaxQuantity)
            {
                target.MaxQuantity = lotSizeFilter.MaxQuantity;
                updated = true;
            }
        }
        if (minNotionalFilter != null && target.MinNotional != minNotionalFilter.MinNotional)
        {
            target.MinNotional = minNotionalFilter.MinNotional;
            updated = true;
        }

        return updated;
    }

    /// <summary>
    /// Calculates precision based on stepSize
    /// </summary>
    /// <param name="stepSize">Step size</param>
    /// <returns></returns>
    private static int CalculatePrecision(decimal stepSize)
    {
        var stepSizeStr = stepSize.ToString(CultureInfo.InvariantCulture);
        var decimalPlaces = stepSizeStr.Length - stepSizeStr.IndexOf('.') - 1;
        return decimalPlaces > 0 ? decimalPlaces : 0;
    }

    /// <summary>
    /// Maps SymbolStatus returned from Binance to corresponding internal SymbolStatus enumeration
    /// </summary>
    /// <param name="binanceStatus">Status of symbol provided by Binance</param>
    /// <returns></returns>
    private static SymbolStatus MapSymbolStatus(BinanceSymbolStatus binanceStatus)
    {
        return binanceStatus switch
        {
            BinanceSymbolStatus.Trading => SymbolStatus.Active,
            BinanceSymbolStatus.Halt or BinanceSymbolStatus.EndOfDay or BinanceSymbolStatus.Break
                => SymbolStatus.Suspended,
            _ => SymbolStatus.Suspended
        };
    }

    /// <summary>
    /// Maps ContractType returned from Binance to corresponding internal ContractType enumeration
    /// </summary>
    /// <param name="binanceContractType">Symbol's contract type provided by Binance</param>
    /// <returns></returns>
    private static ContractType MapContractType(BinanceContractType? binanceContractType)
    {
        if (binanceContractType == null)
        {
            return ContractType.Spot;
        }

        return binanceContractType switch
        {
            BinanceContractType.Perpetual or BinanceContractType.PerpetualDelivering
                => ContractType.Perpetual,
            BinanceContractType.CurrentQuarter or BinanceContractType.CurrentQuarterDelivering
                => ContractType.CurrentQuarter,
            BinanceContractType.NextQuarter or BinanceContractType.NextQuarterDelivering
                => ContractType.NextQuarter,
            BinanceContractType.CurrentMonth => ContractType.CurrentQuarter,
            BinanceContractType.NextMonth => ContractType.NextQuarter,
            _ => ContractType.Perpetual
        };
    }
}
