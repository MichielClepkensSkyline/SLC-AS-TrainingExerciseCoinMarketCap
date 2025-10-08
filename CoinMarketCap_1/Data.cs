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
            string protocolVersion = "1.0.0.1_DIS";

            if (!dms.ProtocolExists(protocolName, protocolVersion))
            {
                engine.Log($"[ERROR] No protocol with this name and version found");
                return;
            }

            IEnumerable<IDmsElement> elements = dms.GetElements().Where(e => e.Protocol.Name == protocolName && e.Protocol.Version == protocolVersion);
            foreach (var element in elements)
            {
                try
                {
                    object latestListings = element.GetTable(100);
                    engine.Log($"[{element.Name}] Latest Listings = {latestListings}");
                }
                catch (Exception ex)
                {
                    engine.Log($"[ERROR] Failed to get Latest listings from {element.Name}: {ex.Message}");
                }

            }
        }
    }
}
