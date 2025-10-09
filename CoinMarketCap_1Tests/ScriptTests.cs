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
	using Skyline.DataMiner.Utils.SecureCoding.SecureIO;
	using static System.Net.WebRequestMethods;

	[TestClass]
	public class ScriptTests
	{
		[TestMethod]
		public void RunTest()
		{
			Assert.Fail();
		}

		[TestMethod]
		public void GetActiveElementsForSpecificProtocolTest_ValidInputs_ReturnsActiveElementsForProtocol()
		{
			string protocolName = "Exercise HTTP CoinMarketCap Tajana";
			Script script = new Script();
			Mock<IDms> mockDms = new Mock<IDms>();

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

			mockDms.Setup(m => m.GetElements()).Returns(elements);

			// Act
			List<IDmsElement> result = script.GetActiveElementsForSpecificProtocol(mockDms.Object, protocolName);

			// Assert
			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public void GetActiveElementsForSpecificProtocolTest_NoActiveElements_ReturnsEmptyList()
		{
			string protocolName = "Exercise HTTP CoinMarketCap Tajana";
			Script script = new Script();
			Mock<IDms> mockDms = new Mock<IDms>();

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

			mockDms.Setup(m => m.GetElements()).Returns(elements);

			// Act
			List<IDmsElement> result = script.GetActiveElementsForSpecificProtocol(mockDms.Object, protocolName);

			// Assert
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void GetActiveElementsForSpecificProtocolTest_NoElementsForProtocol_ReturnsEmptyList()
		{
			string protocolName = "Exercise HTTP CoinMarketCap Tajana";
			string otherProtocol = "Starlink";
			Script script = new Script();
			Mock<IDms> mockDms = new Mock<IDms>();

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

			mockDms.Setup(m => m.GetElements()).Returns(elements);

			// Act
			List<IDmsElement> result = script.GetActiveElementsForSpecificProtocol(mockDms.Object, protocolName);

			// Assert
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void MakeCsvForOneElementTest()
		{
			Assert.Fail();
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

		[TestMethod]
		public void WriteRowsTest()
		{
			Assert.Fail();
		}
	}
}