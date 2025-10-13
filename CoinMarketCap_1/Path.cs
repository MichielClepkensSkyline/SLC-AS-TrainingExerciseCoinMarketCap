namespace CoinMarketCap_1
{
    using System.IO;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

    public class Path
    {
        public SecurePath CreatePath(IEngine engine, string elementName)
        {
            int paramId = 2;
            string folderName = engine.GetScriptParam(paramId).Value;

            if (string.IsNullOrWhiteSpace(folderName))
            {
                engine.ExitFail("Folder name was not entered correctly");
            }

            SecurePath directoryPath = SecurePath.ConstructSecurePath("C:\\Skyline DataMiner\\Documents\\SLC-AS-TrainingExerciseCoinMarketCap",folderName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            SecurePath filePath = SecurePath.ConstructSecurePath(directoryPath, $"{elementName}.csv");

            return filePath;
        }
    }
}
