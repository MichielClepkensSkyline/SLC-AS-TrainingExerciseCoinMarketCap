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
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

	[TestClass]
	public class ScriptTests
	{
		[TestMethod]
		public void MakeCsvForOneElement_WritesCsvFile_TempFolder()
		{
			// Arrange
			var script = new Script();
			var engine = new Mock<IEngine>();
			var element = new Mock<IDmsElement>();
			var table = new Mock<IDmsTable>();

			var rows = new List<IList<object>>
			{
				new List<object>
				{
					"1",
					"Bitcoin",
					"BTC",
					44204.0,
					18000000,
					1,
					44205.0,
					"45000",
					"0.5",
					"5%",
					"850B",
					"PlatformX",
					"21000000",
					"40%",
					"2B",
					"Key1",
				},
			};
			var dataDict = new Dictionary<string, object[]> { { "1", rows[0].ToArray() } };
			table.Setup(t => t.GetData(It.IsAny<int>())).Returns(dataDict);

			element.Setup(e => e.GetTable(It.IsAny<int>())).Returns(table.Object);
			element.Setup(e => e.Name).Returns("BitcoinElement");

			string folderName = "SLC-AS-TrainingExerciseCoinMarketCap";
			string tempFolder = SecurePath.ConstructSecurePath($"C:\\Skyline DataMiner\\Documents", folderName);
			Directory.CreateDirectory(tempFolder);

			engine.Setup(x => x.GetScriptParam("Folder Name").Value).Returns(folderName);

			// Act
			script.MakeCsvForOneElement(engine.Object, element.Object);

			// Assert
			string expectedFile = SecurePath.ConstructSecurePath(tempFolder, "BitcoinElement.csv");
			Assert.IsTrue(File.Exists(expectedFile));

			string content = File.ReadAllText(expectedFile);
			Assert.IsTrue(content.Contains("Bitcoin"));
			Assert.IsTrue(content.Contains("BTC"));

			// Cleanup
			Directory.Delete(tempFolder, true);
		}

		[TestMethod]
		public void MakeCsvForOneElement_EmptyData_LogsInfo()
		{
			// Arrange
			var script = new Script();
			var engine = new Mock<IEngine>();
			var element = new Mock<IDmsElement>();
			var table = new Mock<IDmsTable>();

			var data = new Dictionary<string, object[]>();
			table.Setup(t => t.GetData(It.IsAny<int>())).Returns(data);
			element.Setup(e => e.GetTable(It.IsAny<int>())).Returns(table.Object);
			element.Setup(e => e.Name).Returns("EmptyElement");

			string folderName = "SLC-AS-TrainingExerciseCoinMarketCap";
			engine.Setup(x => x.GetScriptParam("Folder Name").Value).Returns(folderName);

			// Act
			script.MakeCsvForOneElement(engine.Object, element.Object);

			// Assert
			engine.Verify(e => e.GenerateInformation(It.Is<string>(msg => msg.Contains("EmptyElement"))), Times.Once);
		}

		[TestMethod]
		public void MakeCsvForOneElement_NullData_LogsInfo()
		{
			// Arrange
			var script = new Script();
			var engine = new Mock<IEngine>();
			var element = new Mock<IDmsElement>();
			var table = new Mock<IDmsTable>();

			// Null data
			table.Setup(t => t.GetData(It.IsAny<int>())).Returns((Dictionary<string, object[]>)null);
			element.Setup(e => e.GetTable(It.IsAny<int>())).Returns(table.Object);
			element.Setup(e => e.Name).Returns("NullDataElement");

			string folderName = "SLC-AS-TrainingExerciseCoinMarketCap";
			engine.Setup(x => x.GetScriptParam("Folder Name").Value).Returns(folderName);

			// Act
			script.MakeCsvForOneElement(engine.Object, element.Object);

			// Assert
			engine.Verify(e => e.GenerateInformation(It.Is<string>(msg => msg.Contains("NullDataElement"))), Times.Once);
		}

		[TestMethod]
		public void GetActiveElementsForSpecificProtocolTest_ValidInputs_ReturnsActiveElementsForProtocol()
		{
			string protocolName = "Exercise HTTP CoinMarketCap Tajana";
			Script script = new Script();
			Mock<IDms> dms = new Mock<IDms>();

			Mock<IDmsElement> activeElement1 = new Mock<IDmsElement>();
			activeElement1.Setup(a => a.Protocol.Name).Returns(protocolName);
			activeElement1.Setup(a => a.State).Returns(ElementState.Active);
			Mock<IDmsElement> activeElement2 = new Mock<IDmsElement>();
			activeElement2.Setup(a => a.Protocol.Name).Returns(protocolName);
			activeElement2.Setup(a => a.State).Returns(ElementState.Active);

			Mock<IDmsElement> stoppedElement = new Mock<IDmsElement>();
			stoppedElement.Setup(s => s.Protocol.Name).Returns(protocolName);
			stoppedElement.Setup(s => s.State).Returns(ElementState.Stopped);

			List<IDmsElement> elements = new List<IDmsElement>()
			{
				activeElement1.Object,
				activeElement2.Object,
				stoppedElement.Object,
			};

			dms.Setup(m => m.GetElements()).Returns(elements);

			// Act
			List<IDmsElement> result = script.GetActiveElementsForSpecificProtocol(dms.Object, protocolName);

			// Assert
			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public void GetActiveElementsForSpecificProtocolTest_NoActiveElements_ReturnsEmptyList()
		{
			string protocolName = "Exercise HTTP CoinMarketCap Tajana";
			Script script = new Script();
			Mock<IDms> dms = new Mock<IDms>();

			Mock<IDmsElement> stoppedElement1 = new Mock<IDmsElement>();
			stoppedElement1.Setup(s => s.Protocol.Name).Returns(protocolName);
			stoppedElement1.Setup(s => s.State).Returns(ElementState.Stopped);

			Mock<IDmsElement> stoppedElement2 = new Mock<IDmsElement>();
			stoppedElement2.Setup(s => s.Protocol.Name).Returns(protocolName);
			stoppedElement2.Setup(s => s.State).Returns(ElementState.Stopped);

			Mock<IDmsElement> maskedElement = new Mock<IDmsElement>();
			maskedElement.Setup(s => s.Protocol.Name).Returns(protocolName);
			maskedElement.Setup(s => s.State).Returns(ElementState.Masked);

			List<IDmsElement> elements = new List<IDmsElement>()
			{
				stoppedElement1.Object,
				stoppedElement2.Object,
				maskedElement.Object,
			};

			dms.Setup(m => m.GetElements()).Returns(elements);

			// Act
			List<IDmsElement> result = script.GetActiveElementsForSpecificProtocol(dms.Object, protocolName);

			// Assert
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void GetActiveElementsForSpecificProtocolTest_NoElementsForProtocol_ReturnsEmptyList()
		{
			string protocolName = "Exercise HTTP CoinMarketCap Tajana";
			string otherProtocol = "Starlink";
			Script script = new Script();
			Mock<IDms> dms = new Mock<IDms>();

			Mock<IDmsElement> activeElement1 = new Mock<IDmsElement>();
			activeElement1.Setup(a => a.Protocol.Name).Returns(otherProtocol);
			activeElement1.Setup(a => a.State).Returns(ElementState.Active);
			Mock<IDmsElement> activeElement2 = new Mock<IDmsElement>();
			activeElement2.Setup(a => a.Protocol.Name).Returns(otherProtocol);
			activeElement2.Setup(a => a.State).Returns(ElementState.Active);

			List<IDmsElement> elements = new List<IDmsElement>()
			{
				activeElement1.Object,
				activeElement2.Object,
			};

			dms.Setup(m => m.GetElements()).Returns(elements);

			// Act
			List<IDmsElement> result = script.GetActiveElementsForSpecificProtocol(dms.Object, protocolName);

			// Assert
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void FormPathTest_ValidInput_ReturnsSecurePath()
		{
			// Arrange
			Script script = new Script();
			Mock<IEngine> engine = new Mock<IEngine>();
			string folderName = "SLC-AS-TrainingExerciseCoinMarketCap";
			string elementName = "HTTP CoinMarketCap Tajana";
			string expected = SecurePath.ConstructSecurePath("C:\\Skyline DataMiner\\Documents", folderName, $"{elementName}.csv");

			engine.Setup(x => x.GetScriptParam("Folder Name").Value).Returns(folderName);

			// Act
			SecurePath securePath = script.FormPath(engine.Object, elementName);

			// Assert
			Assert.IsNotNull(securePath);
			Assert.AreEqual(expected, securePath.ToString());
			Assert.IsTrue(Directory.Exists(SecurePath.ConstructSecurePath("C:\\Skyline DataMiner\\Documents", folderName)));
		}

		[TestMethod]
		public void FormPathTest_EmptyFolderName_ExitFail()
		{
			// Arrange
			Script script = new Script();
			Mock<IEngine> engine = new Mock<IEngine>();
			engine.Setup(x => x.GetScriptParam("Folder Name").Value).Returns(string.Empty);

			// Assert
			var ex = Assert.ThrowsException<ArgumentException>(() =>
			{
				script.FormPath(engine.Object, "HTTP CoinMarketCap Tajana");
			});
		}
	}
}