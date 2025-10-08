using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Core.DataMinerSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinMarketCap_1
{
	internal class CoinMarketCapElement
	{
		private readonly IEngine engine;
		private readonly IDmsElement element;

		public CoinMarketCapElement(IEngine engine, IDmsElement coinMarketCapElement)
		{
			this.engine = engine;
			element = coinMarketCapElement;

			// = engine.GetDummy("coinMarketCap");
			if (!element.IsStartupComplete())
			{
				throw new ArgumentException("CoinMarketCap element is not active");
			}
		}

		public string GetName()
		{
			return element.Name;
		}

		public IDictionary<string, object[]> GetCryptocurrenciesTable()
		{
			IDmsTable table = element.GetTable(200);
			IDictionary<string, object[]> tabledata = table.GetData();
			return tabledata;
		}

		/*public void GetLastListings()
		{
			engine.Log($"Test", LogType.Error, 1);
			engine.Log("Script geraakt tot het ophalen van de tabel");
			int activeCryptocurrencies = Convert.ToInt32(element.GetParameter(600));
			engine.GenerateInformation(Convert.ToString(activeCryptocurrencies));
		}*/
	}
}
