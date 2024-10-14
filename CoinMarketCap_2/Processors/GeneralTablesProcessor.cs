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
			var categoriesTable = GetTableValues();

			if (categoriesTable != null)
			{
				var csvData = CsvDataExporterHelper.BuildCsv(categoriesTable, _engine, _elementTableConfigDto);

				CsvDataExporterHelper.ExportCsvToFile(filePath, csvData, _engine);

				_engine.Log($"Data successfully exported to CSV at {filePath}");
			}
			else
			{
				_engine.Log("No table data available to export.");
			}
		}

		private object[][] GetTableValues()
		{
			var dataMinerAgent = _engine.GetDms().GetAgent(_elementTableConfigDto.AgentId);
			var element = dataMinerAgent.GetElement(new DmsElementId($"{dataMinerAgent.Id}/{_elementTableConfigDto.ElementId}"));

			if (element == null)
			{
				return new object[0][];
			}

			return element.GetTable(_elementTableConfigDto.TableId).GetRows();
		}
	}
}
