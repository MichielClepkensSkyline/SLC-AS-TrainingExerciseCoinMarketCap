using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinMarketCap_1
{
	public class LatestListing
	{
		public string ID { get; set; }

		public string Name { get; set; }

		public string Symbol { get; set; }

		public DateTime? DateAdded { get; set; }

		public string CirculatingSupply { get; set; }

		public string Rank { get; set; }

		public DateTime? LastUpdated { get; set; }

		public string QuotePrice { get; set; }

		public string OneHourChange { get; set; }

		public string VolumeChange24h { get; set; }

		public string MarketCap { get; set; }

		public string PlatformName { get; set; }

		public string MaximumSupply { get; set; }

		public string MarketCapDominance { get; set; }

		public string Volume24h { get; set; }

		public string DisplayKey { get; set; }
	}
}
