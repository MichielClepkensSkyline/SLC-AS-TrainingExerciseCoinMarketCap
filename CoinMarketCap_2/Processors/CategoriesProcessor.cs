namespace CoinMarketCap_2.Processors
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using CoinMarketCap_2.Dtos;
	using CoinMarketCap_2.Helpers;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	public class CategoriesProcessor
	{
		private readonly IEngine _engine;
		private readonly IDmsElement _coinMarketCapCategories;

		public CategoriesProcessor(IEngine engine, IDmsElement coinMarketCapCategories)
		{
			_engine = engine;
			_coinMarketCapCategories = coinMarketCapCategories;
		}

		public void HandleExtractAndPrepareData(string filePath)
		{
			var categoriesTable = GetCategoriesTable();

			if (categoriesTable != null)
			{
				var csvData = CsvDataExporterHelper.BuildCsv(categoriesTable, new List<int> { 8, 9 });

				csvData = GetColumnDisplayNamesCsvFormat() + '\n' + csvData;

				CsvDataExporterHelper.ExportCsvToFile(filePath, csvData, _engine);

				_engine.Log($"Data successfully exported to CSV at {filePath}");
			}
			else
			{
				_engine.Log("No global metrics data available to export.");
			}
		}

		private object[][] GetCategoriesTable()
		{
			if (_coinMarketCapCategories == null)
			{
				return new object[0][];
			}

			return _coinMarketCapCategories.GetTable(70).GetRows();
		}

		private string GetColumnDisplayNamesCsvFormat()
		{
			var element = _engine.FindElement(161, _coinMarketCapCategories.Id);

			var protocol = element.Protocol;

			var parameters = protocol.Parameters.Where(p => p.ID >= 71 && p.ID <= 80).OrderBy(p => p.ID).Select(p => p.DisplayName);

			return string.Join(",", parameters);
		}
	}
}
