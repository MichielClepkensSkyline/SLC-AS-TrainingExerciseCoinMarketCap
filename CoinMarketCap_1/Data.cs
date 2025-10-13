namespace CoinMarketCap_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;
	using CsvHelper;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

	public class Data
	{
		private const string ProtocolName = "Exercise HTTP CoinMarketCap Tajana";

		private const string ProtocolVersion = "Production";

		private const int LatestListingTableId = 100;

		public List<IDmsElement> GetActiveElementsForSpecificProtocol(IDms dms, IEngine engine)
		{
			List <IDmsElement> elements = dms.GetElements().Where(p => p.Protocol.Name == ProtocolName && p.State == ElementState.Active && p.Protocol.Version == ProtocolVersion).ToList();

			if (elements == null || elements.Count == 0)
			{
				engine.GenerateInformation($"RunSafe|No active elements found for protocol '{ProtocolName}'.");
			}

			return elements;
		}

		public void MakeCsvForOneElement(IEngine engine, IDmsElement element)
		{
			IDmsTable lastListingTable = element.GetTable(LatestListingTableId);

			if (lastListingTable == null)
			{
				engine.GenerateInformation($"MakeCsvForOneElement|Table {lastListingTable.Element.Name}  is null.");
				return;
			}

			var data = lastListingTable.GetData();

			if (data == null || data.Count == 0)
			{
				engine.GenerateInformation($"MakeCsvForOneElement|No data found for element '{element.Name}'.");
				return;
			}

			List<LatestListing> listings = new List<LatestListing>();

			foreach (var row in data.Values)
			{
				var listing = MapToLatestListing(row);
				if (listing != null)
				{
					listings.Add(listing);
				}
			}

			if (listings.Count == 0)
			{
				engine.GenerateInformation($"MakeCsvForOneElement|No valid rows mapped for element '{element.Name}'.");
				return;
			}

			Path path = new Path();
			SecurePath filePath = path.FormPath(engine, element.Name);

			using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteHeader<LatestListing>();
				csv.NextRecord();
				csv.WriteRecords(listings);
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
