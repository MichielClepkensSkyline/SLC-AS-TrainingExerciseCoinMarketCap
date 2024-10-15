namespace CoinMarketCap_2.Processors
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Text;
	using System.Threading.Tasks;

	using CoinMarketCap_2.Dtos;
	using CoinMarketCap_2.Helpers;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	public class GeneralTablesProcessor
	{
		private readonly IEngine _engine;
		private readonly ElementTableConfigDto _elementTableConfigDto;

		public GeneralTablesProcessor(IEngine engine, ElementTableConfigDto elementTableConfigDto)
		{
			_engine = engine;
			_elementTableConfigDto = elementTableConfigDto;
		}

		public void HandleExtractAndPrepareTableData(string filePath)
		{
			var dataMinerAgent = _engine.GetDms().GetAgent(_elementTableConfigDto.AgentId);
			var element = dataMinerAgent.GetElement(new DmsElementId($"{dataMinerAgent.Id}/{_elementTableConfigDto.ElementId}"));

			if (element.State != ElementState.Active)
			{
				_engine.GenerateInformation($"Element '{element.Name}' is not active so no data was exported.");
				return;
			}

			var categoriesTable = GetTableValues(element);

			if (categoriesTable == null || categoriesTable.Length == 0)
			{
				_engine.GenerateInformation($"Element '{element.Name}' has no available data to export.");
				return;
			}

			var csvData = CsvDataExporterHelper.BuildCsv(categoriesTable, _engine, _elementTableConfigDto);

			CsvDataExporterHelper.ExportCsvToFile(filePath, csvData, _engine);

			_engine.Log($"Data successfully exported to CSV at {filePath}");
		}

		private object[][] GetTableValues(IDmsElement element)
		{
			if (element == null || element.State != ElementState.Active)
			{
				return new object[0][];
			}

			return element.GetTable(_elementTableConfigDto.TableId).GetRows();
		}
	}
}
