using Skyline.DataMiner.Automation;
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
		private readonly IActionableElement element;

		public CoinMarketCapElement(IEngine engine, IDmsElement coinMarketCapElement)
		{
			this.engine = engine;
			element = engine.GetDummy("coinMarketCap");
			if (!element.IsActive)
			{
				throw new ArgumentException("CoinMarketCap element is not active");
			}
		}

		public void GetLastListings()
		{
			engine.Log("Script geraakt tot het ophalen van de tabel");
			int activeCryptocurrencies = Convert.ToInt32(element.GetParameter(600));
			engine.GenerateInformation(Convert.ToString(activeCryptocurrencies));
		}
	}
}
