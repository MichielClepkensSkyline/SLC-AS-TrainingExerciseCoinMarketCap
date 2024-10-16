namespace CoinMarketCap_1.Dtos
{
	using System;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	internal class GlobalMetricsParametersDto
	{
		public string LastUpdate { get; set; }

		public IDmsStandaloneParameter<double?> BitcoinDominance { get; set; }

		public IDmsStandaloneParameter<double?> BitcoinDominancePercentageChange24H { get; set; }

		public IDmsStandaloneParameter<double?> EthereumDominance { get; set; }

		public IDmsStandaloneParameter<double?> EthereumDominancePercentageChange24H { get; set; }

		public IDmsStandaloneParameter<double?> TotalMarketCap { get; set; }

		public IDmsStandaloneParameter<double?> TotalVolume24H { get; set; }

		public IDmsStandaloneParameter<double?> TotalMarketCapYesterday { get; set; }

		public IDmsStandaloneParameter<double?> TotalMarketCapYesterdayPercentageChange { get; set; }

		public IDmsStandaloneParameter<double?> DecentralizedFinanceTradingVolume24H { get; set; }

		public IDmsStandaloneParameter<double?> DecentralizedFinanceMarketCap { get; set; }

		public IDmsStandaloneParameter<double?> StablecoinTradingVolume24H { get; set; }

		public IDmsStandaloneParameter<double?> StablecoinMarketCap { get; set; }
	}
}
