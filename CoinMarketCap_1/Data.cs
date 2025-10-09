using CsvHelper;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Core.DataMinerSystem.Common;
using Skyline.DataMiner.Core.DataMinerSystem.Common.Selectors;
using Skyline.DataMiner.Net.ReportsAndDashboards;
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
        public enum Tables
        {
            LatestListings = 100,
            Categories = 400,
        }

        public void GetData(IDms dms, IEngine engine)
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
                    if(latestListingsData != null)
                    {
                        StoreDataInCSV(latestListingsData, engine,element.Name);
                    }

                    var categoriesData = ReadDataFormTable((int)Tables.Categories, element, engine);
                    if (categoriesData != null)
                    {
                        // StoreDataInCSV(categoriesData, engine);
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
                engine.Log("Table was not found");
                return null;
            }

            IDictionary<string, object[]> tableData = table.GetData();
            if (tableData == null)
            {
                engine.Log("No data was found");
                return null;
            }

            return tableData;
        }

        public void StoreDataInCSV(IDictionary<string, object[]> tableData, IEngine engine, string elementName)
        {
            string filePath = $"C:\\Skyline DataMiner\\Documents\\SLC-AS-TrainingExerciseCoinMarketCap\\{elementName}.csv";

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                List<LatestListings> listings = new List<LatestListings>();

                foreach (var row in tableData)
                {
                    object[] cols = row.Value;
                    engine.Log($"Date: {cols[9]}");
                    try
                    {
                        listings.Add(new LatestListings
                        {
                            Id = row.Key,
                            Symbol = cols[2]?.ToString(),
                            Name = cols[1]?.ToString(),
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

                foreach(var listing in listings)
                {
                    engine.Log($"ID: {listing.Id}");
                    engine.Log($"Name: {listing.Id}");
                    engine.Log($"Rank: {listing.Id}");
                    engine.Log($"Price in USD: {listing.Id}");
                    engine.Log($"Market Cap: {listing.Id}");
                    engine.Log($"One Hour Change: {listing.Id}");
                    engine.Log($"One Day Volume: {listing.Id}");
                    engine.Log($"Last Updated: {listing.Id}");
                    engine.Log($"Display Key: {listing.DisplayKey}");
                }

                csv.WriteRecords(listings);
            }
        }
    }
}
