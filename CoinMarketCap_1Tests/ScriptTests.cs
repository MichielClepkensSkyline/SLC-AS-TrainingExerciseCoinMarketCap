namespace CoinMarketCap_1.Tests
{
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
			Script script = new Script();
			Mock<IEngine> engine = new Mock<IEngine>();

			// Act
			List<TableRow> result = script.FillRecords(tabledata, engine.Object);

			// Assert
			Assert.IsTrue(result.Count == 0);
		}
	}
}