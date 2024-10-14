namespace CoinMarketCap_2.DataProcessors
{
    using System;

    using CoinMarketCap_2.Dtos;
    using CoinMarketCap_2.Helpers;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;

    public class GlobalMetricsDataProcessor
	{
		private readonly IEngine _engine;
		private readonly IDmsElement _coinMarketCapGlobalMetrics;

		public GlobalMetricsDataProcessor(IEngine engine, IDmsElement coinMarketCapGlobalMetrics)
		{
			_engine = engine;
			_coinMarketCapGlobalMetrics = coinMarketCapGlobalMetrics;
		}

		public void HandleExtractAndPrepareData(string filePath)
		{
			var globalMetricsParameters = GetGlobalMetricsParameters();

			if (globalMetricsParameters != null)
			{
				var csvData = CsvDataExporterHelper.BuildCsvData(new[] { globalMetricsParameters }, _engine);

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