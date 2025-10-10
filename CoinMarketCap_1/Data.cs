namespace CoinMarketCap_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using CsvHelper;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Core.DataMinerSystem.Common.Selectors;
	using Skyline.DataMiner.Net.ReportsAndDashboards;
	using Skyline.DataMiner.Net.SLSearch.Misc;

	public class Data
    {
        public enum Tables
        {
            LatestListings = 100,
        }

        public void StoreData(IDms dms, IEngine engine)
        {
            string protocolName = "Exercise HTTP CoinMarketCap Emir";
            string protocolVersion = "Production";

            if (!dms.ProtocolExists(protocolName, protocolVersion))
            {
                engine.Log($"[ERROR] No protocol with this name and version was found");
                return;
            }

            IEnumerable<IDmsElement> elements = dms.GetElements().Where(e => e.Protocol.Name == protocolName && e.Protocol.Version == protocolVersion);
            foreach (IDmsElement element in elements)
            {
                try
                {
                    var latestListingsData= ReadDataFormTable((int)Tables.LatestListings, element, engine);
                    if (latestListingsData != null)
                    {
                        StoreDataInCSV(latestListingsData, engine,element.Name);
                    }
                    else
                    {
                        engine.ExitFail("There was an issue with reading the data from the table");
                    }
                }
                catch (Exception ex)
                {
                    engine.Log($"[ERROR] Failed to get and store data from {element.Name}: {ex.Message}");
                }
            }
        }

        public IDictionary<string, object[]> ReadDataFormTable(int tableId, IDmsElement element, IEngine engine)
        {
            IDmsTable table = element.GetTable(tableId);
            if (table == null)
            {
                engine.Log($"[ERROR] Table with the designated id was not found in {element.Name}");
                return null;
            }

            IDictionary<string, object[]> tableData = table.GetData();
            if (tableData == null)
            {
                engine.Log($"[ERROR] No data was found in the table with the deisgnated id in the {element.Name} element");
                return null;
            }

            return tableData;
        }

        public void StoreDataInCSV(IDictionary<string, object[]> listingsTableData, IEngine engine, string elementName)
        {
            string filePath = CreatePath(engine, elementName);

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                List<LatestListings> listings = new List<LatestListings>();

                foreach (var row in listingsTableData)
                {
                    object[] cols = row.Value;
                    try
                    {
                        listings.Add(new LatestListings
                        {
                            Id = row.Key,
                            Name = cols[1]?.ToString(),
                            Symbol = cols[2]?.ToString(),
                            CoinMarketCapRank = Convert.ToInt32(cols[3]),
                            CirculatingSupply = Convert.ToDouble(cols[4]),
                            PriceInUSD = Convert.ToDouble(cols[5]),
                            MarketCapInUSD = Convert.ToDouble(cols[6]),
                            OneHourChange = Convert.ToDouble(cols[7]),
                            OneDayVolumeChange = Convert.ToDouble(cols[8]),
                            LastUpdated = DateTime.FromOADate(Convert.ToDouble(cols[9])),
                            DisplayKey = cols[10]?.ToString(),
                        });
                    }
                    catch (Exception ex)
                    {
                        engine.Log($"[ERROR] Failed to map row '{row.Key}': {ex.Message}");
                    }
                }

                csv.WriteRecords(listings);
                engine.Log($"File saved succesfully at {filePath}");
            }
        }

        public string CreatePath(IEngine engine, string elementName)
        {
            string folderName = engine.GetScriptParam("FolderName").Value;

            if (string.IsNullOrWhiteSpace(folderName))
            {
                engine.ExitFail("Folder name was not entered correctly");
            }

            string directoryPath = $"C:\\Skyline DataMiner\\Documents\\SLC-AS-TrainingExerciseCoinMarketCap\\{folderName}";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = $"{directoryPath}\\{elementName}.csv";

            return filePath;
        }
    }
}
