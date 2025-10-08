using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Core.DataMinerSystem.Common;
using Skyline.DataMiner.Core.DataMinerSystem.Common.Selectors;
using Skyline.DataMiner.Net.ReportsAndDashboards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinMarketCap_1
{
    public class Data
    {
        public void GetData(IDms dms, IEngine engine)
        {
            string protocolName = "Exercise HTTP CoinMarketCap Emir";
            string protocolVersion = "Production";


            if (!dms.ProtocolExists(protocolName, protocolVersion))
            {
                engine.Log($"[ERROR] No protocol with this name and version was found");
                return;
            }

            IEnumerable<IDmsElement> elements = dms.GetElements().Where(e => e.Protocol.Name == protocolName && e.Protocol.Version== protocolVersion);
            foreach (IDmsElement element in elements)
            {
                try
                {
                    int latestListingsTableId = 100;
                    var latestListingsData= ReadDataFormTable(latestListingsTableId, element, engine);
                    if(latestListingsData != null)
                    {
                        StoreDataInCSV(latestListingsData, engine);
                    }

                    int categoriesTableId = 400;
                    ReadDataFormTable(categoriesTableId, element, engine);
                    var categoriesData = ReadDataFormTable(categoriesTableId, element, engine);
                    if (categoriesData != null)
                    {
                        StoreDataInCSV(categoriesData, engine);
                    }
                }
                catch (Exception ex)
                {
                    engine.Log($"[ERROR] Failed to get store data from {element.Name}: {ex.Message}");
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

        public void StoreDataInCSV(IDictionary<string, object[]> tableData, IEngine engine)
        {

        }
    }
}
