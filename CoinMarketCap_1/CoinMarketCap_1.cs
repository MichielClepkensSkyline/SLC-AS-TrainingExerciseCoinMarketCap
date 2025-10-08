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
	using CsvHelper;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Automation.Logging;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Core.DataMinerSystem.Common.Selectors;
	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
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
			var dms = engine.GetDms();

			if (dms == null)
			{
				return;
			}

			string[] columnNames = new[] { "ID", "Name", "Symbol", "Date Added", "Circulating supply", "Rank", "Last Updated", "Quote Price", "1h Change", "Volume Change (24h)", "Market Cap", "Platform Name", "Maximum supply", "Market Cap Dominance", "Volume (24h)", "Display Key" };

			List<IDmsElement> elements = GetActiveElementsForSpecificProtocol(dms, protocolName);

			var latestListingTableId = 100;
			foreach (var element in elements)
			{
				MakeCsvForOneElement(engine, element, latestListingTableId, columnNames);
			}
		}

		public SecurePath FormPath(IEngine engine, string elementName)
		{
			string filePath = $"C:\\Skyline DataMiner\\Documents\\{engine.GetScriptParam("Folder Name").Value}\\{elementName}.csv";
			SecurePath securePath = SecurePath.CreateSecurePath(filePath);

			//engine.Log($"File path: {filePath}", LogType.Debug, 0);
			return securePath;
		}

		public List<IDmsElement> GetActiveElementsForSpecificProtocol(IDms dms, string protocolName)
		{
			return dms.GetElements().Where(p => p.Protocol.Name == protocolName && p.State == ElementState.Active).ToList();
		}

		public void MakeCsvForOneElement(IEngine engine, IDmsElement element, int latestListingTableId, string[] columnNames)
		{
			var csvBuilder = new StringBuilder();
			IDmsTable lastListingTable = element.GetTable(latestListingTableId);
			var data = lastListingTable.GetData();
			SecurePath filePath = FormPath(engine, element.Name);
			using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))

			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				foreach (var colName in columnNames)
				{
					csv.WriteField(colName);
				}


				csv.NextRecord();

				foreach (var row in data.Values)
				{
					int colIndex = 0;

					foreach (var item in row)
					{
						if ((colIndex == 3 || colIndex == 6) && double.TryParse(item?.ToString(), out double oaDate))
						{
							DateTime dt = DateTime.FromOADate(oaDate);
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
}