namespace CoinMarketCap_1Tests
{
	using System;
	using CoinMarketCap_1.Helpers;
	using FluentAssertions;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	[TestClass]
	public class CsvDataExporterHelperTests
	{
		[TestMethod]
		public void ProcessStandaloneParametersTest_ShouldFormatDoubleCorrectly()
		{
			// Arrange
			var mockParam = new Mock<IDmsStandaloneParameter<double?>>();
			mockParam.Setup(p => p.GetDisplayValue()).Returns("1234567.89");

			var testObject = new TestClass { DoubleParameter = mockParam.Object };

			// Act
			var result = CsvDataExporterHelper.ProcessStandaloneParameters(
				testObject, typeof(TestClass).GetProperties());

			// Assert
			result.Should().Contain("1 234 568");
		}

		private class TestClass
		{
			public IDmsStandaloneParameter<double?> DoubleParameter { get; set; }
		}
	}
}