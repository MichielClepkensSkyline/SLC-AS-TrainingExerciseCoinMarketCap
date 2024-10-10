using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoinMarketCap_2.Dtos;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Core.DataMinerSystem.Common;

namespace CoinMarketCap_2
{
	public class GlobalMetricsDataExporter
	{
		private readonly IEngine _engine;
		private readonly IDmsElement _coinMarketCapGlobalMetrics;

		public GlobalMetricsDataExporter(IEngine engine, IDmsElement coinMarketCapGlobalMetrics)
		{
			_engine = engine;
			_coinMarketCapGlobalMetrics = coinMarketCapGlobalMetrics;
		}

		public void ExportDataToCsv(string filePath)
		{
			var globalMetricsParameters = GetGlobalMetricsParameters();

			if (globalMetricsParameters != null)
			{
				_engine.Log($"Last Update: {globalMetricsParameters.LastUpdate?.GetDisplayValue()}");
				_engine.Log($"Bitcoin Dominance: {globalMetricsParameters.BitcoinDominance?.GetDisplayValue()}");
				_engine.Log($"Bitcoin Dominance % Change 24H: {globalMetricsParameters.BitcoinDominancePercentageChange24H?.GetDisplayValue()}");
				_engine.Log($"Ethereum Dominance: {globalMetricsParameters.EthereumDominance?.GetDisplayValue()}");
				_engine.Log($"Ethereum Dominance % Change 24H: {globalMetricsParameters.EthereumDominancePercentageChange24H?.GetDisplayValue()}");
				_engine.Log($"Total Market Cap: {globalMetricsParameters.TotalMarketCap?.GetDisplayValue()}");
				_engine.Log($"Total Volume 24H: {globalMetricsParameters.TotalVolume24H?.GetDisplayValue()}");
				_engine.Log($"Total Market Cap Yesterday: {globalMetricsParameters.TotalMarketCapYesterday?.GetDisplayValue()}");
				_engine.Log($"Total Market Cap Yesterday % Change: {globalMetricsParameters.TotalMarketCapYesterdayPercentageChange?.GetDisplayValue()}");
				_engine.Log($"DeFi Trading Volume 24H: {globalMetricsParameters.DecentralizedFinanceTradingVolume24H?.GetDisplayValue()}");
				_engine.Log($"DeFi Market Cap: {globalMetricsParameters.DecentralizedFinanceMarketCap?.GetDisplayValue()}");
				_engine.Log($"Stablecoin Trading Volume 24H: {globalMetricsParameters.StablecoinTradingVolume24H?.GetDisplayValue()}");
				_engine.Log($"Stablecoin Market Cap: {globalMetricsParameters.StablecoinMarketCap?.GetDisplayValue()}");
			}
		}

		private GlobalMetricsParametersDto GetGlobalMetricsParameters()
		{
			if (_coinMarketCapGlobalMetrics == null)
			{
				return null;
			}

			return new GlobalMetricsParametersDto
			{
				LastUpdate = _coinMarketCapGlobalMetrics.GetStandaloneParameter<DateTime?>(49),
				BitcoinDominance = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(51),
				EthereumDominance = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(52),
				BitcoinDominancePercentageChange24H = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(53),
				EthereumDominancePercentageChange24H = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(54),
				TotalMarketCap = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(56),
				TotalVolume24H = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(57),
				TotalMarketCapYesterday = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(58),
				TotalMarketCapYesterdayPercentageChange = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(59),
				DecentralizedFinanceTradingVolume24H = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(60),
				StablecoinTradingVolume24H = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(61),
				DecentralizedFinanceMarketCap = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(62),
				StablecoinMarketCap = _coinMarketCapGlobalMetrics.GetStandaloneParameter<double?>(63),
			};
		}
	}
}
