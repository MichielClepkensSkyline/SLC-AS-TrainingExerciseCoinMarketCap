/*
****************************************************************************
*  Copyright (c) 2025,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

07/10/2025	1.0.0.1		SKF, Skyline	Initial version
****************************************************************************
*/

namespace CoinMarketCap_1
{
	using CsvHelper;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Utils.SecureCoding.SecureIO;
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string ProtocolName = "Exercise HTTP CoinMarketCap Sofian";
		private const string ProtocolVersion = "Production";
		private const int ScriptParamId = 3;
		private const string BasePath = "C:\\Skyline DataMiner\\Documents\\SLC-AS-TrainingExerciseCoinMarketCap";
		private const string FileExtension = ".csv";
		private static IDms dms;
		private readonly List<CoinMarketCapElement> coinMarketCapElements = new List<CoinMarketCapElement>();
		private List<TableRow> records;

		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			try
			{
				if (!InitializeDMS(engine))
				{
					engine.ExitFail("Initialization engine failed");
				}

				// Get all elements with the CoinMarketCap protocol
				GetCoinMarketCapElements(engine);

				// Check if there are elements with the CoinMarketCap protocol
				if (coinMarketCapElements.Count == 0)
				{
					engine.ExitFail($"No elements of type {ProtocolName} correctly initialized");
				}

				// Loop through all CoinMarketCapElements and make csv file
				MakeCsvFiles(engine);
			}
			catch (Exception e)
			{
				engine.ExitFail($"ERROR: {e}");
			}
		}

		private bool InitializeDMS(IEngine engine)
		{
			try
			{
				dms = engine.GetDms();
				if (dms == null)
				{
					engine.Log("Initializing DMS failed: dms is null", LogType.Error, 0);
					return false;
				}
			}
			catch (Exception e)
			{
				engine.Log("Initializing DMS failed: " + e, LogType.Error, 0);
				return false;
			}

			return true;
		}

		private void GetCoinMarketCapElements(IEngine engine)
		{
			List<IDmsElement> elements = (List<IDmsElement>)dms.GetElements();
			foreach (IDmsElement element in elements)
			{
				if (element.Protocol.Name == ProtocolName && element.Protocol.Version == ProtocolVersion)
				{
					InitializeElement(engine, element);
				}
			}
		}

		private void InitializeElement(IEngine engine, IDmsElement coinMarketCapElement)
		{
			try
			{
				coinMarketCapElements.Add(new CoinMarketCapElement(engine, coinMarketCapElement));
			}
			catch (Exception e)
			{
				engine.Log("Initializing CoinMarketCap element failed: " + e, LogType.Error, 0);
			}
		}

		private void MakeCsvFiles(IEngine engine)
		{
			foreach (CoinMarketCapElement element in coinMarketCapElements)
			{
				// Make csv file
				using (CsvWriter csv = MakeCsvWriter(engine, element))
				{
					// Get cryptocurrencies table of the CoinMarketCap element
					IDictionary<string, object[]> tabledata = element.GetCryptocurrenciesTable();

					// Fill records list with rows from the table
					records = element.FillRecords(tabledata, engine);

					// Write records to file
					if (records != null)
					{
						csv.WriteRecords(records);
					}
				}
			}
		}

		private CsvWriter MakeCsvWriter(IEngine engine, CoinMarketCapElement element)
		{
			// Check folder
			string folderPath = engine.GetScriptParam(ScriptParamId).Value;
			SecurePath secureFolderPath = SecurePath.ConstructSecurePath(BasePath, folderPath);
			if (!Directory.Exists(secureFolderPath))
			{
				Directory.CreateDirectory(secureFolderPath);
			}

			// Make secure path
			string filePath = element.GetName() + FileExtension;
			SecurePath securePath = SecurePath.ConstructSecurePath(secureFolderPath, filePath);

			// Make csv writer
			StreamWriter writer = new StreamWriter(securePath);
			CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
			return csvWriter;
		}
	}
}