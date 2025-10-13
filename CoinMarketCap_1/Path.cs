namespace CoinMarketCap_1
{
	using System;
	using System.IO;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

	public class Path
	{
		public SecurePath FormPath(IEngine engine, string elementName)
		{
			int scriptParamId = 2;
			string folderName = engine.GetScriptParam(scriptParamId).Value;

			if (String.IsNullOrWhiteSpace(folderName))
			{
				engine.ExitFail("FormPath|Script parameter 'Folder Name' is null or whitespace.");
			}

			string folderPath = SecurePath.ConstructSecurePath("C:\\Skyline DataMiner\\Documents\\SLC-AS-TrainingExerciseCoinMarketCap", folderName);

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			SecurePath securePath = SecurePath.ConstructSecurePath(folderPath, $"{elementName}.csv");
			return securePath;
		}
	}
}
