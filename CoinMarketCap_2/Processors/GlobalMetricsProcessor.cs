﻿namespace CoinMarketCap_2.DataProcessors
{
	using System;

	using CoinMarketCap_2.Dtos;
	using CoinMarketCap_2.Helpers;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	public class GlobalMetricsProcessor
	{
		private readonly IEngine _engine;
		private readonly IDmsElement _coinMarketCapGlobalMetrics;
		private const int AgentId = 161;
		private const int ElementId = 13;

		public GlobalMetricsProcessor(IEngine engine)
		{
			_engine = engine;
			_coinMarketCapGlobalMetrics = _engine.GetDms().GetElement(new DmsElementId($"{AgentId}/{ElementId}"));
		}

		public void HandleExtractAndPrepareData(string filePath)
		{
			var globalMetricsParameters = GetGlobalMetricsParameters();

			if (globalMetricsParameters != null)
			{
				var csvData = CsvDataExporterHelper.BuildCsv(globalMetricsParameters);

				CsvDataExporterHelper.ExportCsvToFile(filePath, csvData, _engine, true);

				_engine.Log($"Data successfully exported to CSV at {filePath}");
			}
			else
			{
				_engine.Log("No global metrics data available to export.");
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