namespace CoinMarketCap_1Tests
{
	using CoinMarketCap_1.Helpers;

	using FluentAssertions;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	using System;
	using System.Globalization;
	using System.Threading;

	[TestClass]
	public class CsvDataExporterHelperTests
	{
		[TestMethod]
		[DataRow("en-US", "1 234 568,This is a string.")]
		[DataRow("fr-FR", "1 234 568,This is a string.")]
		[DataRow("de-DE", "1 234 568,This is a string.")]
		public void ProcessStandaloneParameters_ShouldFormatValuesCorrectly_ForDifferentCultures(string cultureName, string expectedResult)
		{
			// Arrange
			Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureName);

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
			Console.WriteLine($"Expected: {expectedResult}");
			Console.WriteLine($"Actual: {result}");

			result.Should().Be(expectedResult);
		}

		private class TestClass
		{
			public IDmsStandaloneParameter<double?> DoubleParameter { get; set; }

			public IDmsStandaloneParameter<string> StringParameter { get; set; }
		}
	}
}
