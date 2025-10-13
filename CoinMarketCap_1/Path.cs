using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinMarketCap_1
{
	public class Path
	{

		public SecurePath FormPath(IEngine engine, string elementName)
		{
			string folderName = engine.GetScriptParam("Folder Name").Value;

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
