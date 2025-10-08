using CsvHelper;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Core.DataMinerSystem.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinMarketCap_1
{
	internal class CoinMarketCapElement
	{
		private const int TablePID = 200;
		private readonly IEngine engine;
		private readonly IDmsElement element;

		public CoinMarketCapElement(IEngine engine, IDmsElement coinMarketCapElement)
		{
			this.engine = engine;
			element = coinMarketCapElement;

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
			IDmsTable table = element.GetTable(TablePID);
			IDictionary<string, object[]> tabledata = table.GetData();
			return tabledata;
		}

		public CsvWriter MakeCsvWriter()
		{
			// TODO: add foutenafhandeling of zoiets
			StreamWriter writer = new StreamWriter($"C:\\Skyline DataMiner\\Documents\\SLC-AS-TrainingExerciseCoinMarketCap\\test_{this.GetName()}.csv");
			CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
			return csvWriter;
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
