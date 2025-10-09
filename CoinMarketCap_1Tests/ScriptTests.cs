using CoinMarketCap_1;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Core.DataMinerSystem.Common;
using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.WebRequestMethods;

namespace CoinMarketCap_1.Tests
{
	[TestClass]
	public class ScriptTests
	{

		private Mock<IEngine> engine;
		private Script script;

		[TestMethod]
		public void RunTest()
		{
			Assert.Fail();
		}

		[TestMethod]
		public void GetActiveElementsForSpecificProtocolTest()
		{

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
			this.script = new Script();
			this.engine = new Mock<IEngine>();
			string folderName = "SLC-AS-TrainingExerciseCoinMarketCap";
			string elementName = "HTTP CoinMarketCap Tajana";
			string expected = SecurePath.ConstructSecurePath("C:\\Skyline DataMiner\\Documents", folderName, $"{elementName}.csv");

			this.engine.Setup(x => x.GetScriptParam("Folder Name").Value).Returns(folderName);

			// Act
			SecurePath securePath = this.script.FormPath(this.engine.Object, elementName);

			//Assert
			Assert.IsNotNull(securePath);
			Assert.AreEqual(expected, securePath.ToString());
			Assert.IsTrue(Directory.Exists(SecurePath.ConstructSecurePath("C:\\Skyline DataMiner\\Documents", folderName)));

		}

		[TestMethod]
		public void FormPathTest_EmptyFolderName_ExitFail()
		{
			// Arrange
			this.script = new Script();
			this.engine = new Mock<IEngine>();
			this.engine.Setup(x => x.GetScriptParam("Folder Name").Value).Returns(string.Empty);

			// Assert
			var ex = Assert.ThrowsException<ArgumentException>(() =>
			{
				this.script.FormPath(this.engine.Object, "HTTP CoinMarketCap Tajana");
			});
		}

		[TestMethod]
		public void WriteRowsTest()
		{
			Assert.Fail();
		}
	}
}