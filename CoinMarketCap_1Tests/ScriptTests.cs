namespace CoinMarketCap_1.Tests
{
	/// <summary>
	/// Unit Test Class.
	/// </summary>
	[TestClass]
	public class ScriptTests
	{
		/// <summary>
		/// Test method for FillRecords where tabledata is null.
		/// </summary>
		[TestMethod]
		public void FillRecordsTest()
		{
			// Arrange
			Dictionary<string, object[]>? tabledata = null;
			Script script = new Script();

			// Act
			List<TableRow> result = script.FillRecords(tabledata);

			// Assert
			Assert.IsTrue(result.Count == 0);
		}
	}
}