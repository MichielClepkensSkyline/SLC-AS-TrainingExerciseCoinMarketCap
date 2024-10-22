namespace CoinMarketCap_1Tests
{
	using CoinMarketCap_1.Helpers;
	using FluentAssertions;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	[TestClass]
	public class CsvDataExporterHelperTests
	{
		[TestMethod]
		public void ProcessStandaloneParametersTest_ShouldFormatValuesCorrectly()
		{
			// Arrange
			var mockParamDouble = new Mock<IDmsStandaloneParameter<double?>>();
			var mockParamString = new Mock<IDmsStandaloneParameter<string>>();
			mockParamDouble.Setup(p => p.GetDisplayValue()).Returns("1234567.89");
			mockParamString.Setup(p => p.GetDisplayValue()).Returns("This is a string.");

			var testObject = new TestClass
			{
				DoubleParameter = mockParamDouble.Object,
				StringParameter = mockParamString.Object,
			};

			// Act
			var result = CsvDataExporterHelper.ProcessStandaloneParameters(
				testObject, typeof(TestClass).GetProperties());

			// Assert
			result.Should().Contain("1 234 568,This is a string.");
		}

		private class TestClass
		{
			public IDmsStandaloneParameter<double?> DoubleParameter { get; set; }

			public IDmsStandaloneParameter<string> StringParameter { get; set; }
		}
	}
}