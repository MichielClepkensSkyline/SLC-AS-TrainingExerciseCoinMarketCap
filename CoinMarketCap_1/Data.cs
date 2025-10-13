using CsvHelper;

using Skyline.DataMiner.Analytics.GenericInterface.QueryBuilder;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Core.DataMinerSystem.Automation;
using Skyline.DataMiner.Core.DataMinerSystem.Common;
using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinMarketCap_1
{
	public class Data
	{
		private readonly string[] columnNames = new[]
		{
			"ID", "Name", "Symbol", "Date Added", "Circulating supply", "Rank", "Last Updated", "Quote Price",
			"1h Change", "Volume Change (24h)", "Market Cap", "Platform Name", "Maximum supply",
			"Market Cap Dominance", "Volume (24h)", "Display Key",
		};

		public readonly string protocolName = "Exercise HTTP CoinMarketCap Tajana";

		private readonly string protocolVersion = "Production";

		private int latestListingTableId = 100;

		public List<IDmsElement> GetActiveElementsForSpecificProtocol(IDms dms)
		{
			return dms.GetElements().Where(p => p.Protocol.Name == protocolName && p.State == ElementState.Active && p.Protocol.Version == protocolVersion).ToList();
		}

		public void MakeCsvForOneElement(IEngine engine, IDmsElement element)
		{
			var csvBuilder = new StringBuilder();
			IDmsTable lastListingTable = element.GetTable(latestListingTableId);
			//add a null check for the table 
			var data = lastListingTable.GetData();

			if (data == null || data.Count == 0)
			{
				engine.GenerateInformation($"MakeCsvForOneElement|No data found for element '{element.Name}'.");
				return;
			}

			Path path = new Path();
			SecurePath filePath = path.FormPath(engine, element.Name);

			using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))

			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				foreach (var colName in columnNames)
				{
					csv.WriteField(colName);
				}

				csv.NextRecord();

				WriteRows(csv, data.Values);
			}
		}

		public void WriteRows(CsvWriter csv, IEnumerable<IList<object>> data)
		{
			foreach (var row in data)
			{
				if (row == null || row.Count == 0)
				{
					continue;
				}

				var listing = MapToLatestListing(row);

				if (listing == null)
					continue;

				csv.WriteField(listing.ID);
				csv.WriteField(listing.Name);
				csv.WriteField(listing.Symbol);
				csv.WriteField(listing.DateAdded?.ToString("yyyy-MM-dd HH:mm:ss"));
				csv.WriteField(listing.CirculatingSupply);
				csv.WriteField(listing.Rank);
				csv.WriteField(listing.LastUpdated?.ToString("yyyy-MM-dd HH:mm:ss"));
				csv.WriteField(listing.QuotePrice);
				csv.WriteField(listing.OneHourChange);
				csv.WriteField(listing.VolumeChange24h);
				csv.WriteField(listing.MarketCap);
				csv.WriteField(listing.PlatformName);
				csv.WriteField(listing.MaximumSupply);
				csv.WriteField(listing.MarketCapDominance);
				csv.WriteField(listing.Volume24h);
				csv.WriteField(listing.DisplayKey);

				csv.NextRecord();
			}
		}

		public LatestListing MapToLatestListing(IList<object> row)
		{
			if (row == null || row.Count < 16)
			{
				return null;
			}

			LatestListing listing = new LatestListing
			{
				ID = row[0]?.ToString(),
				Name = row[1]?.ToString(),
				Symbol = row[2]?.ToString(),
				DateAdded = DateTime.FromOADate((double)row[3]),
				CirculatingSupply = row[4]?.ToString(),
				Rank = row[5]?.ToString(),
				LastUpdated = DateTime.FromOADate((double)row[6]),
				QuotePrice = row[7]?.ToString(),
				OneHourChange = row[8]?.ToString(),
				VolumeChange24h = row[9]?.ToString(),
				MarketCap = row[10]?.ToString(),
				PlatformName = row[11]?.ToString(),
				MaximumSupply = row[12]?.ToString(),
				MarketCapDominance = row[13]?.ToString(),
				Volume24h = row[14]?.ToString(),
				DisplayKey = row[15]?.ToString(),
			};

			return listing;
		}
	}
}
