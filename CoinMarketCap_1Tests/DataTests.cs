// <copyright file="DataTests.cs" company="Skyline Communications">
// Copyright (c) Skyline Communications. All rights reserved.
// </copyright>

namespace CoinMarketCap_1.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using CoinMarketCap_1;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Core.DataMinerSystem.Common.Selectors;

	[TestClass]
	public class DataTests
	{

		public readonly Mock<IEngine> mockEngine = new Mock<IEngine>();

		[TestMethod]
		public void CreatePathTest_CreateNewDirectory()
		{
			// Arrange
			var mockParam = new Mock<ScriptParam>();

			mockParam.Setup(p => p.Value).Returns("TestFolder");

			this.mockEngine.Setup(e => e.GetScriptParam("FolderName")).Returns(mockParam.Object);

			string elementName = "TestElement";
			var data = new Data();

			string expectedDirectory = @"C:\Skyline DataMiner\Documents\SLC-AS-TrainingExerciseCoinMarketCap\TestFolder";
			string expectedFilePath = Path.Combine(expectedDirectory, $"{elementName}.csv");

			if (Directory.Exists(expectedDirectory))
			{
				Directory.Delete(expectedDirectory, recursive: true);
			}

			// Act
			string resultPath = data.CreatePath(this.mockEngine.Object, elementName);

			// Assert
			Assert.AreEqual(expectedFilePath, resultPath);
			Assert.IsTrue(Directory.Exists(expectedDirectory), "Directory should be created.");
		}

		[TestMethod]
		public void CreatePath_EmptyFolderName_ExitsWithFail()
		{
			var mockParam = new Mock<ScriptParam>();
			mockParam.Setup(p => p.Value).Returns(string.Empty);

			this.mockEngine.Setup(e => e.GetScriptParam("FolderName")).Returns(mockParam.Object);

			var data = new Data();

			// Act
			data.CreatePath(this.mockEngine.Object, "TestElement");

			// Assert
			this.mockEngine.Verify(e => e.ExitFail("Folder name was not entered correctly"), Times.Once);
		}

		[TestMethod]
		public void ReadDataFromTable_TableIsNull()
		{
			// Arrange
			var mockElement = new Mock<IDmsElement>();
			mockElement.Setup(e => e.Name).Returns("MockElement");
			mockElement.Setup(e => e.GetTable(It.IsAny<int>())).Returns(value: null);

			var data = new Data();

			// Act
			var result = data.ReadDataFormTable(100, mockElement.Object, this.mockEngine.Object);

			// Assert
			Assert.IsNull(result);
			this.mockEngine.Verify(e => e.Log("[ERROR] Table with the designated id was not found in MockElement"), Times.Once);
		}

		[TestMethod]
		public void ReadDataFromTable_TableDataIsNull()
		{
			// Arrange
			var mockTable = new Mock<IDmsTable>();
			mockTable.Setup(t => t.GetData(It.IsAny<int>())).Returns((IDictionary<string, object[]>)null);

			var mockElement = new Mock<IDmsElement>();
			mockElement.Setup(e => e.Name).Returns("MockElement");
			mockElement.Setup(e => e.GetTable(It.IsAny<int>())).Returns(mockTable.Object);

			var data = new Data();

			// Act
			var result = data.ReadDataFormTable(100, mockElement.Object, this.mockEngine.Object);

			// Assert
			Assert.IsNull(result);
			this.mockEngine.Verify(e => e.Log("[ERROR] No data was found in the table with the deisgnated id in the MockElement element"), Times.Once);
		}

		public void ReadDataFormTable_ValidTableAndData_ReturnsData()
		{
			// Arrange
			var expectedData = new Dictionary<string, object[]>
			{
				{ "1", new object[] { "BTC", 50000.0 } },
				{ "2", new object[] { "ETH", 4000.0 } },
			};

			var mockTable = new Mock<IDmsTable>();
			mockTable.Setup(t => t.GetData(It.IsAny<int>())).Returns((IDictionary<string, object[]>)null);

			var mockElement = new Mock<IDmsElement>();
			mockElement.Setup(e => e.GetTable(It.IsAny<int>())).Returns(mockTable.Object);
			mockElement.Setup(e => e.Name).Returns("MockElement");

			var mockEngine = new Mock<IEngine>();

			var data = new Data();

			// Act
			var result = data.ReadDataFormTable(100, mockElement.Object, mockEngine.Object);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("BTC", result["1"][0]);
			mockEngine.Verify(e => e.Log(It.IsAny<string>()), Times.Never);
		}
	}
}