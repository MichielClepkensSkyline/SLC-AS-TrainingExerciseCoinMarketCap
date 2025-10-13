namespace CoinMarketCap_1
{
    using System;

    public class LatestListings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public int CoinMarketCapRank { get; set; }

        public double CirculatingSupply { get; set; }

        public double PriceInUSD { get; set; }

        public double MarketCapInUSD { get; set; }

        public double OneHourChange { get; set; }

        public double OneDayVolumeChange { get; set; }

        public DateTime LastUpdated { get; set; }

        public string DisplayKey { get; set; }
    }
}
