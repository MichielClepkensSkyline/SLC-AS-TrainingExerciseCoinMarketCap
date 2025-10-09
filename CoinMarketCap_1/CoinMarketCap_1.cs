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

11/01/2024	1.0.0.1		TSA, Skyline	Initial version
****************************************************************************
*/

namespace CoinMarketCap_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;
	using CsvHelper;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Automation.Logging;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Core.DataMinerSystem.Common.Selectors;
	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		private readonly string[] columnNames = new[]
		{
			"ID", "Name", "Symbol", "Date Added", "Circulating supply", "Rank", "Last Updated", "Quote Price",
			"1h Change", "Volume Change (24h)", "Market Cap", "Platform Name", "Maximum supply",
			"Market Cap Dominance", "Volume (24h)", "Display Key",
		};

		private int latestListingTableId = 100;

		public void Run(IEngine engine)
		{
			try
			{
				RunSafe(engine);
			}
			catch (ScriptAbortException)
			{
				throw;
			}
			catch (ScriptForceAbortException)
			{
				throw;
			}
			catch (ScriptTimeoutException)
			{
				throw;
			}
			catch (InteractiveUserDetachedException)
			{
				throw;
			}
			catch (Exception e)
			{
				engine.Log("Run|Something went wrong: " + e);
			}
		}

		private void RunSafe(IEngine engine)
		{
			string protocolName = "Exercise HTTP CoinMarketCap Tajana";
			var dms = EnsureDms(engine);

			List<IDmsElement> elements = GetActiveElementsForSpecificProtocol(dms, protocolName);

			if (elements == null || elements.Count == 0)
			{
				engine.GenerateInformation($"RunSafe|No active elements found for protocol '{protocolName}'.");
				return;
			}

			foreach (var element in elements)
			{
				MakeCsvForOneElement(engine, element);
			}
		}

		private IDms EnsureDms(IEngine engine)
		{
			try
			{
				var dms = engine.GetDms();
				if (dms == null)
				{
					engine.ExitFail("DMS connection could not be established.");
				}

				return dms;
			}
			catch (Exception ex)
			{
				engine.ExitFail("EnsureDms|Exception while getting DMS: " + ex.Message);
				return null;
			}
		}

		public List<IDmsElement> GetActiveElementsForSpecificProtocol(IDms dms, string protocolName)
		{
			return dms.GetElements().Where(p => p.Protocol.Name == protocolName && p.State == ElementState.Active).ToList();
		}

		public void MakeCsvForOneElement(IEngine engine, IDmsElement element)
		{
			var csvBuilder = new StringBuilder();
			IDmsTable lastListingTable = element.GetTable(latestListingTableId);
			var data = lastListingTable.GetData();

			if (data == null || data.Count == 0)
			{
				engine.GenerateInformation($"MakeCsvForOneElement|No data found for element '{element.Name}'.");
				return;
			}

			SecurePath filePath = FormPath(engine, element.Name);

			using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))

			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				foreach (var colName in columnNames)
				{
					csv.WriteField(colName);
				}

				csv.NextRecord();

				WriteRows(csv, data.Values);
			}
		}

		public SecurePath FormPath(IEngine engine, string elementName)
		{
			string folderName = engine.GetScriptParam("Folder Name").Value;

			if (String.IsNullOrWhiteSpace(folderName))
			{
				engine.ExitFail("FormPath|Script parameter 'Folder Name' is null or whitespace.");
			}

			string folderPath = SecurePath.ConstructSecurePath("C:\\Skyline DataMiner\\Documents", folderName);

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			SecurePath securePath = SecurePath.ConstructSecurePath(folderPath, $"{elementName}.csv");
			return securePath;
		}

		public void WriteRows(CsvWriter csv, IEnumerable<IList<object>> data)
		{
			foreach (var row in data)
			{
				if (row == null || row.Count == 0)
				{
					continue;
				}

				int colIndex = 0;

				foreach (var item in row)
				{
					if ((colIndex == 3 || colIndex == 6) && double.TryParse(item?.ToString(), out double rawDate))
					{
						DateTime dt = DateTime.FromOADate(rawDate);
						csv.WriteField(dt.ToString("yyyy-MM-dd HH:mm:ss"));
					}
					else
					{
						csv.WriteField(item?.ToString());
					}

					colIndex++;
				}

				csv.NextRecord();
			}
		}
	}
}