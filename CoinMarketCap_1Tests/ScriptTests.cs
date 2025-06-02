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
            IDictionary<string, object[]> tableData = null; // testiramo sa null
            string fakeFilePath = "Z:\\fakepath\\notexist.csv";
            string[] columns = null;

            // Act & Assert
            NUnit.Framework.Assert.Throws<DirectoryNotFoundException>(() =>
                script.ExportTableData(columns, tableData, fakeFilePath, false));
        }

        [TestMethod()]
        public void PrepareAndExportMarketDataTable_nullData()
        {
            // Arrange
            var script = new Script();
            IDictionary<string, object[]> tableData = null; // testiramo sa null
            string fakeFilePath = "Z:\\fakepath\\notexist.csv";

            // Act
            try
            {
                script.PrepareAndExportMarketDataTable(tableData, fakeFilePath);
            }
            // Assert
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    Assert.Fail("NullReferenceException was thrown.");
                }
                else
                {
                    Assert.IsTrue(true);
                }
            }
        }

        [TestMethod()]
        public void PrepareAndExportCryptoListingTable_nullData()
        {
           // Arrange
            var script = new Script();
            IDictionary<string, object[]> tableData = null; // testiramo sa null
            string fakeFilePath = "Z:\\fakepath\\notexist.csv";

            // Act
            try
            {
                script.PrepareAndExportCryptoListingTable(tableData, fakeFilePath);
            }
            // Assert
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    Assert.Fail("NullReferenceException was thrown.");
                }
                else
                {
                    Assert.IsTrue(true);
                }
            }
        }
    }
}