using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoinMarketCap_1
{
	internal class TableRow
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Symbol { get; set; }

		public int NumMarketPairs { get; set; }

		public int CmcRank { get; set; }

		public double CirculatingSupply { get; set; }

		public double TotalSupply { get; set; }

		public double MaxSupply { get; set; }

		public double LastUpdated { get; set; }

		public double DateAdded { get; set; }

		public double TvlRatio { get; set; }

		public string PlatformName { get; set; }

		public string Quote { get; set; }

		public double Price { get; set; }

		public double Volume24h { get; set; }

		public double VolumeChange24h { get; set; }

		public double MarketCap { get; set; }

		public double MarketCapDominance { get; set; }

		public double PercentChange1h { get; set; }

		public double PercentChange24h { get; set; }

		public double PercentChange7d { get; set; }

		public string DisplayKey { get; set; }
	}
}