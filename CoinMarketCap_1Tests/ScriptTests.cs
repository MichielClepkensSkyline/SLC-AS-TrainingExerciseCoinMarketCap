namespace CoinMarketCap_1Tests
{
	using CoinMarketCap_1;
	using Moq;
	using Skyline.DataMiner.Automation;

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
			Mock<IEngine> engine = new ();

			// Act
			List<TableRow> result = CoinMarketCapElement.FillRecords(tabledata, engine.Object);

			// Assert
			Assert.IsTrue(result.Count == 0);
		}
	}
}