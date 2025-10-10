namespace CoinMarketCap_1
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	public class CoinMarketCapElement
	{
		private const int TablePID = 200;
		private readonly IEngine engine;
		private readonly IDmsElement element;

		public CoinMarketCapElement(IEngine engine, IDmsElement coinMarketCapElement)
		{
			this.engine = engine;
			element = coinMarketCapElement;

			if (element.State != ElementState.Active)
			{
				throw new ArgumentException("CoinMarketCap element is not active");
			}
		}

		public static List<TableRow> FillRecords(IDictionary<string, object[]> tabledata, IEngine engine)
		{
			List<TableRow> records = new List<TableRow>();
			if (tabledata != null)
			{
				foreach (KeyValuePair<string, object[]> data in tabledata)
				{
					object[] row = data.Value;
					try
					{
						records.Add(new TableRow
						{
							Id = Convert.ToString(row[0]),
							Name = Convert.ToString(row[1]),
							Symbol = Convert.ToString(row[2]),
							NumMarketPairs = Convert.ToInt32(row[3]),
							CmcRank = Convert.ToInt32(row[4]),
							CirculatingSupply = Convert.ToDouble(row[5]),
							TotalSupply = Convert.ToDouble(row[6]),
							MaxSupply = Convert.ToDouble(row[7]),
							LastUpdated = DateTime.FromOADate(Convert.ToDouble(row[8])),
							DateAdded = DateTime.FromOADate(Convert.ToDouble(row[9])),
							TvlRatio = Convert.ToDouble(row[10]),
							PlatformName = Convert.ToString(row[11]),
							Quote = Convert.ToString(row[12]),
							Price = Convert.ToDouble(row[13]),
							Volume24h = Convert.ToDouble(row[14]),
							VolumeChange24h = Convert.ToDouble(row[15]),
							MarketCap = Convert.ToDouble(row[16]),
							MarketCapDominance = Convert.ToDouble(row[17]),
							PercentChange1h = Convert.ToDouble(row[18]),
							PercentChange24h = Convert.ToDouble(row[19]),
							PercentChange7d = Convert.ToDouble(row[20]),
							DisplayKey = Convert.ToString(row[21]),
						});
					}
					catch (Exception ex)
					{
						engine.Log($"[ERROR] Failed to fill in row (key: '{data.Key}': {ex.Message}", LogType.Error, 1);
					}
				}
			}

			return records;
		}

		public string GetName()
		{
			return element.Name;
		}

		public IDictionary<string, object[]> GetCryptocurrenciesTable()
		{
			IDmsTable table = element.GetTable(TablePID);
			IDictionary<string, object[]> tabledata = table.GetData();
			return tabledata;
		}
	}
}
