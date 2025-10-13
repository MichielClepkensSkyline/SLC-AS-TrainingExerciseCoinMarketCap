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

				int colIndex = 0;

				foreach (var item in row)
				{
					if ((colIndex == 3 || colIndex == 6) && double.TryParse(item?.ToString(), out double rawDate))
					{
						DateTime dt = DateTime.FromOADate(rawDate);
						csv.WriteField(dt.ToString("yyyy-MM-dd HH:mm:ss"));
					}
					else
					{
						csv.WriteField(item?.ToString());
					}

					colIndex++;
				}

				csv.NextRecord();
			}
		}
	}
}
