namespace CoinMarketCap_1.Helpers
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;

	using CoinMarketCap_1.Dtos;

	using Skyline.DataMiner.Automation;

	public class CsvDataExporterHelper
	{
		public static string BuildCsv<T>(T data, IEngine engine = null, TableConfigDto config = null)
		{
			var sb = new StringBuilder();

			if (data is object[][] tableData)
			{
				sb.AppendLine(GetColumnDisplayNamesCsvFormat(engine, config));
				sb.AppendLine(ProcessTable(tableData, config));
			}
			else
			{
				var properties = typeof(T).GetProperties();
				sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));
				sb.AppendLine(ProcessStandaloneParameters(data, properties));
			}

			return sb.ToString();
		}

		public static void ExportCsvToFile(string filePath, string csvData, IEngine engine, bool append = false)
		{
			if (!filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
			{
				filePath += ".csv";
			}

			int retryCount = 3;
			int delayBetweenRetries = 5000;
			bool writeSuccessful = false;

			for (int i = 0; i < retryCount; i++)
			{
				try
				{
					if (append && File.Exists(filePath))
					{
						var dataLines = csvData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Skip(1);
						csvData = string.Join(Environment.NewLine, dataLines);
						File.AppendAllText(filePath, csvData + Environment.NewLine);
					}
					else
					{
						File.WriteAllText(filePath, csvData);
					}

					writeSuccessful = true;
					break;
				}
				catch (IOException)
				{
					if (i < retryCount - 1)
					{
						System.Threading.Thread.Sleep(delayBetweenRetries);
					}
				}
			}

			if (!writeSuccessful && engine != null)
			{
				engine.ExitFail("Failed to write to the file after multiple attempts. Exiting with failure.");
				engine.Log("File write operation failed after multiple attempts. Please ensure the CSV file is not open or being used by another process, then try running the script again.");
				engine.GenerateInformation("CSV file needs to not interact with other processes to be able to write to it.");
			}
		}

		internal static string ProcessStandaloneParameters<T>(T data, PropertyInfo[] properties)
		{
			return string.Join(",", properties.Select(p =>
			{
				var value = p.GetValue(data);
				var displayValueMethod = value?.GetType().GetMethod("GetDisplayValue");

				if (displayValueMethod != null)
				{
					value = displayValueMethod.Invoke(value, null);
				}

				return FormatCsvValue(value);
			}));
		}

		private static string GetColumnDisplayNamesCsvFormat(IEngine engine, TableConfigDto elementTableConfig)
		{
			var element = engine.FindElement(elementTableConfig.AgentId, elementTableConfig.ElementId);
			var protocol = element.Protocol;

			var parameters = protocol.Parameters.Where(p => p.ID > elementTableConfig.TableId && p.ID <= elementTableConfig.LastTableColumnId).OrderBy(p => p.ID).Select(p => p.DisplayName);

			return string.Join(",", parameters);
		}

		private static string ProcessTable(object[][] tableData, TableConfigDto config)
		{
			return string.Join(Environment.NewLine, tableData.Select(row => ProcessTableRow(row, config)));
		}

		private static string ProcessTableRow(object[] tableRow, TableConfigDto elementTableConfigDto)
		{
			return string.Join(",", tableRow.Select((value, index) =>
			{
				var isDateColumn = IsDateColumn(index, elementTableConfigDto);
				return FormatCsvValue(value, isDateColumn);
			}));
		}

		private static bool IsDateColumn(int index, TableConfigDto config)
		{
			var columnId = index + config.TableId + 1;
			return config.TableDateColumnIds?.Contains(columnId) ?? false;
		}

		private static string FormatCsvValue(object value, bool isDateColumn = false)
		{
			if (double.TryParse(Convert.ToString(value), out double parsedDouble))
			{
				if (isDateColumn)
				{
					try
					{
						var dateTimeValue = DateTime.FromOADate(parsedDouble);
						return dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
					}
					catch (ArgumentException)
					{
						return parsedDouble.ToString("N0").Replace(",", " ");
					}
				}

				return parsedDouble.ToString("N0").Replace(",", " ");
			}

			return Convert.ToString(value);
		}
	}
}
