using System.Reflection;
using System.Text;

namespace CoinMarketCap_1.Tests
{
    [TestClass()]
    public class ScriptTests
    {
        [TestMethod()]
        public void ExportTableData_nonExistentDirectory()
        {
            // Arrange
            var script = new Script();
            string fakeFilePath = "Z:\\fakepath\\notexist.csv";
            string data = null;

            // Act & Assert
            NUnit.Framework.Assert.Throws<DirectoryNotFoundException>(() =>
                script.ExportTableData(data, fakeFilePath, false));
        }

        [TestMethod()]
        public void PrepareMarketDataTable_nullData()
        {
            // Arrange
            var script = new Script();
            IDictionary<string, object[]> tableData = null;

            var displayColumnNames = typeof(MarketTable)
           .GetFields(BindingFlags.Public | BindingFlags.Static)
           .Select(folder => folder.GetValue(null)?.ToString()).ToArray();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Join(",", displayColumnNames));
            var expectedTableContent = stringBuilder.ToString();

            // Act

            var tableContent = script.PrepareMarketDataTable(tableData);

            // Assert
            Assert.AreEqual<string>(tableContent, expectedTableContent);
        }

        [TestMethod()]
        public void PrepareCryptoListingTable_nullData()
        {
            // Arrange
            var script = new Script();
            IDictionary<string, object[]> tableData = null;

            var displayColumnNames = typeof(CryptoListingTable)
           .GetFields(BindingFlags.Public | BindingFlags.Static)
           .Select(folder => folder.GetValue(null)?.ToString()).ToArray();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Join(",", displayColumnNames));
            var expectedTableContent = stringBuilder.ToString();

            // Act
            var tableContent = script.PrepareCryptoListingTable(tableData);
            
            // Assert
            Assert.AreEqual<string>(tableContent, expectedTableContent);
        }
    }
}