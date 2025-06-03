namespace CoinMarketCap_1.Tests
{
    [TestClass]
    public class ScriptTests
    {
        [TestMethod]
        public void CreatePath_ValidInputs_ReturnsCorrectPath()
        {
            // Arrange
            var script = new Script();
            string inputFolderName = "CSV files";
            string inputElementName = "CoinMarketCap element 1";
            string expected = "C:\\Skyline DataMiner\\Documents\\CSV files\\CoinMarketCap element 1.csv";

            // Act
            string actual = script.CreatePath(inputFolderName, inputElementName);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CreatePath_EmptyFolder_UsesDefault()
        {
            // Arrange
            var script = new Script();
            string folder = string.Empty;
            string element = "CoinMarketCap element 1";
            string expected = "C:\\Skyline DataMiner\\Documents\\CSV files\\CoinMarketCap element 1.csv";

            // Act
            string actual = script.CreatePath(folder, element);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreatePath_EmptyElement_ThrowsException()
        {
            // Arrange
            var script = new Script();

            // Act
            script.CreatePath("SomeFolder", string.Empty);
        }
       
        [TestMethod]
        public void WriteTableDataToCsvfile_ValidData_WritesCsv()
        {
            var script = new Script();
            var data = new Dictionary<string, object[]>
            {
                { 
                    "row1", 
                    new object[] 
                    { 
                        1, "Bitcoin", "BTC", DateTime.Now.ToOADate(), 1000, 200, 50000, 1, 2, 3
                    } 
                }
            };

            var columns = new List<string>
            {
                "Rank", "Name", "Symbol", "Last Update", "Market Cap", "Circulating Supply", "Price", "1h%", "24h%", "7d%",
            };

            string tempFile = Path.GetTempFileName();

            try
            {
                script.WriteTableDataToCsvfile(data, tempFile, columns, 3);

                var lines = File.ReadAllLines(tempFile);
                Assert.AreEqual(2, lines.Length); // Header + 1 row
                Assert.IsTrue(lines[0].StartsWith("Rank,Name,Symbol"));
                Assert.IsTrue(lines[1].Contains("Bitcoin"));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void WriteTableDataToCsvfile_EmptyTable_Throws()
        {
            var script = new Script();
            var data = new Dictionary<string, object[]>();
            var columns = new List<string> { "A", "B", "C", "D" };

            script.WriteTableDataToCsvfile(data, "dummy.csv", columns, 3);
        }
    }
}