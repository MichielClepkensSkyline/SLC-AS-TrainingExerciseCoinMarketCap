namespace CoinMarketCap_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using CsvHelper;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

	internal class CoinMarketCapElement
	{
		private const int TablePID = 200;
		private const string ScriptParamName = "folderName";
		private const string BasePath = "C:\\Skyline DataMiner\\Documents\\SLC-AS-TrainingExerciseCoinMarketCap";
		private const string FileExtension = ".csv";
		private readonly IEngine engine;
		private readonly IDmsElement element;

		public CoinMarketCapElement(IEngine engine, IDmsElement coinMarketCapElement)
		{
			this.engine = engine;
			element = coinMarketCapElement;

			if (element.State != ElementState.Active)
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
			// Check folder
			string folderPath = engine.GetScriptParam(ScriptParamName).Value;
			SecurePath secureFolderPath = SecurePath.ConstructSecurePath(BasePath, folderPath);
			if (!Directory.Exists(secureFolderPath))
			{
				Directory.CreateDirectory(secureFolderPath);
			}

			// Make secure path
			string filePath = this.GetName() + FileExtension;
			SecurePath securePath = SecurePath.ConstructSecurePath(secureFolderPath, filePath);

			// Make csv writer
			StreamWriter writer = new StreamWriter(securePath);
			CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
			return csvWriter;
		}
	}
}
