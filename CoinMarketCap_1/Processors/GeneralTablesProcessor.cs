namespace CoinMarketCap_1.Processors
{
	using CoinMarketCap_1.Dtos;
	using CoinMarketCap_1.Helpers;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	public class GeneralTablesProcessor
	{
		private readonly IEngine _engine;
		private readonly ElementTableConfigDto _tableExporterConfig;

		public GeneralTablesProcessor(IEngine engine, ElementTableConfigDto tableExporterConfig)
		{
			_engine = engine;
			_tableExporterConfig = tableExporterConfig;
		}

		public void HandleExtractAndPrepareTableData(string filePath)
		{
			var element = GetElement();

			if (!IsElementActive(element))
				return;

			var tableData = GetTableValues(element);

			if (IsTableEmpty(tableData, element.Name))
				return;

			ExportTableData(tableData, filePath);
		}

		private IDmsElement GetElement()
		{
			var agent = _engine.GetDms()?.GetAgent(_tableExporterConfig.AgentId);
			if (agent == null)
			{
				_engine.Log($"Agent with ID {_tableExporterConfig.AgentId} not found.");
				return null;
			}

			var element = agent.GetElement(new DmsElementId($"{agent.Id}/{_tableExporterConfig.ElementId}"));
			if (element == null)
			{
				_engine.Log($"Element with ID {_tableExporterConfig.ElementId} not found.");
			}

			return element;
		}

		private bool IsElementActive(IDmsElement element)
		{
			if (element == null || element.State != ElementState.Active)
			{
				_engine.GenerateInformation($"Element '{element?.Name ?? "Unknown"}' is not active, so no data was exported.");
				return false;
			}

			return true;
		}

		private bool IsTableEmpty(object[][] tableData, string elementName)
		{
			if (tableData.Length == 0)
			{
				_engine.GenerateInformation($"Element '{elementName}' has no available data to export.");
				return true;
			}

			return false;
		}

		private object[][] GetTableValues(IDmsElement element)
		{
			if (element == null || element.State != ElementState.Active)
			{
				return new object[0][];
			}

			return element.GetTable(_tableExporterConfig.TableId).GetRows();
		}

		private void ExportTableData(object[][] tableData, string filePath)
		{
			var csvData = CsvDataExporterHelper.BuildCsv(tableData, _engine, _tableExporterConfig);
			CsvDataExporterHelper.ExportCsvToFile(filePath, csvData, _engine);
			_engine.Log($"Data successfully exported to CSV at {filePath}");
		}
	}
}
