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
		public static string BuildCsvData<T>(IEnumerable<T> data, IEngine engine)
		{
			var sb = new StringBuilder();
			var properties = typeof(T).GetProperties();

			sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

			foreach (var item in data)
			{
				var row = string.Join(",", properties.Select(p =>
				{
					var value = p.GetValue(item)
							.GetType()
							.GetMethod("GetDisplayValue")
							.Invoke(p.GetValue(item), null);

					if (double.TryParse(value.ToString(), out double parsedDouble))
					{
						return parsedDouble.ToString();
					}

					return value.ToString();
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
