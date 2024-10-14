namespace CoinMarketCap_2.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;

	using Skyline.DataMiner.Automation;

	public class CsvDataExporterHelper
	{
		public static string BuildCsv<T>(T data, List<int> dateColumns = null)
		{
			var sb = new StringBuilder();

			if (data is object[][] tableData)
			{
				foreach (var tableRow in tableData)
				{
					sb.AppendLine(string.Join(",", tableRow.Select((r, index) =>
					{
						bool isDateColumn = dateColumns != null && dateColumns.Contains(index);

						if (double.TryParse(Convert.ToString(r), out double parsedDouble))
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

						return Convert.ToString(r);
					})));
				}
			}
			else
			{
				var properties = typeof(T).GetProperties();
				sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

				var row = string.Join(",", properties.Select(p =>
				{
					var value = p.GetValue(data);
					var displayValueMethod = value?.GetType().GetMethod("GetDisplayValue");

					if (displayValueMethod != null)
					{
						value = displayValueMethod.Invoke(value, null);
					}

					if (double.TryParse(Convert.ToString(value), out double parsedDouble))
					{
						return Convert.ToString(parsedDouble.ToString("N0").Replace(",", " "));
					}

					return Convert.ToString(value);
				}));

				sb.AppendLine(row);
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
	}
}
