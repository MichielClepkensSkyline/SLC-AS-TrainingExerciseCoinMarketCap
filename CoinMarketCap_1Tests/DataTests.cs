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
			mockTable.Setup(t => t.GetData(It.IsAny<int>())).Returns(value: null);

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
			mockTable.Setup(t => t.GetData(It.IsAny<int>())).Returns(value: null);

			var mockElement = new Mock<IDmsElement>();
			mockElement.Setup(e => e.GetTable(It.IsAny<int>())).Returns(mockTable.Object);
			mockElement.Setup(e => e.Name).Returns("MockElement");

			var data = new Data();

			// Act
			var result = data.ReadDataFormTable(100, mockElement.Object, this.mockEngine.Object);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("BTC", result["1"][0]);
			this.mockEngine.Verify(e => e.Log(It.IsAny<string>()), Times.Never);
		}

		[TestMethod]
		public void StoreDataInCSV_ValidData()
		{
			// Arrange
			var data = new Data();

			var testData = new Dictionary<string, object[]>
			{
				{
					"1", new object[] {
					"1", "Bitcoin", "BTC", 1, 19000000.0, 50000.0, 950000000000.0, 1.5, 2.1, DateTime.Now.ToOADate(), "BTC_Display" }
				},
			};

			this.mockEngine.Setup(e => e.GetScriptParam("FolderName").Value).Returns("TestFolder");
			string elementName = "TestElement";

			// Act
			data.StoreDataInCSV(testData, this.mockEngine.Object, elementName);

			// Assert
			this.mockEngine.Verify(e => e.Log(It.Is<string>(msg => msg.Contains("File saved succesfully"))), Times.Once);
		}

		[TestMethod]
		public void StoreDataInCSV_InvalidRow()
		{
			// Arrange
			var data = new Data();

			Dictionary<string, object[]> invaildData = new Dictionary<string, object[]>
			{
				{ "1", new object[] { null, "Invalid", "BTC", "NOT_A_NUMBER", 1, 2, 3, 4, 5, 6, 7 } },
			};

			this.mockEngine.Setup(e => e.GetScriptParam("FolderName").Value).Returns("Test");
			string elementName = "Broken";

			// Act
			data.StoreDataInCSV(invaildData, this.mockEngine.Object, elementName);

			// Assert
			this.mockEngine.Verify(e => e.Log(It.Is<string>(s => s.Contains("[ERROR] Failed to map row"))), Times.Once);
		}

		[TestMethod]
		public void StoreData_ProtocolNotFound()
		{
			// Arrange
			var mockDms = new Mock<IDms>();

			mockDms.Setup(d => d.ProtocolExists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			var data = new Data();

			// Act
			data.StoreData(mockDms.Object,this.mockEngine.Object);

			// Assert
			this.mockEngine.Verify(e => e.Log("[ERROR] No protocol with this name and version was found"), Times.Once);
		}

		[TestMethod]
		public void StoreData_NullTableData_EngineExitFailCalled()
		{
			string protocolName = "Exercise HTTP CoinMarketCap Emir";
			string protocolVersion = "Production";
			var mockDms = new Mock<IDms>();
			var mockElement = new Mock<IDmsElement>();

			mockElement.Setup(e => e.Name).Returns("Element1");
			mockElement.Setup(e => e.Protocol.Name).Returns(protocolName);
			mockElement.Setup(e => e.Protocol.Version).Returns(protocolVersion);

			mockDms.Setup(d => d.ProtocolExists(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			mockDms.Setup(d => d.GetElements()).Returns(new[] { mockElement.Object });

			var data = new Mock<Data>();
			data.CallBase = true;

			data.Setup(s => s.ReadDataFormTable(It.IsAny<int>(), mockElement.Object, mockEngine.Object)).Returns((IDictionary<string, object[]>)null);

			data.Object.StoreData(mockDms.Object, this.mockEngine.Object);

			this.mockEngine.Verify(e => e.ExitFail("There was an issue with reading the data from the table"), Times.Once);
		}

		[TestMethod]
		public void StoreData_ValidElementAndData()
		{
			string protocolName = "Exercise HTTP CoinMarketCap Emir";
			string protocolVersion = "Production";
			var mockDms = new Mock<IDms>();
			var mockElement = new Mock<IDmsElement>();

			mockElement.Setup(e => e.Name).Returns("Element1");
			mockElement.Setup(e => e.Protocol.Name).Returns(protocolName);
			mockElement.Setup(e => e.Protocol.Version).Returns(protocolVersion);

			var fakeData = new Dictionary<string, object[]>
			{
				{ "1", new object[] { "1", "Bitcoin", "BTC", 1, 1000000.0, 50000.0, 900000000.0, 0.5, 0.8, DateTime.Now.ToOADate(), "BTC_Display" } },
			};

			mockDms.Setup(d => d.ProtocolExists(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			mockDms.Setup(d => d.GetElements()).Returns(new[] { mockElement.Object });

			Mock<Data> data = new Mock<Data>();
			data.CallBase = true;

			data.Setup(s => s.ReadDataFormTable(It.IsAny<int>(), mockElement.Object, this.mockEngine.Object)).Returns(fakeData);
			data.Setup(s => s.StoreDataInCSV(fakeData, mockEngine.Object, "Element1"));

			data.Object.StoreData(mockDms.Object, this.mockEngine.Object);

			data.Verify(s => s.StoreDataInCSV(fakeData, this.mockEngine.Object, "Element1"), Times.Once);
		}
	}
}