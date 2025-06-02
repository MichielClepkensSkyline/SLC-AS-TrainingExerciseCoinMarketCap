/*
****************************************************************************
*  Copyright (c) 2024,  Skyline Communications NV  All Rights Reserved.    *
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

11/01/2024	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

namespace CoinMarketCap_1
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;

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
            IDms dms = engine.GetDms();
            var elements = dms.GetElements().Where(x => x.Protocol.Name == "Exercise HTTP CoinMarketCap");
            engine.GenerateInformation(elements.Count().ToString());
            string folderPath = engine.GetScriptParam(2).Value;

            foreach (var element in elements)
            {
                if (element.State.ToString() == "Active")
                {
                    string elementPath = folderPath + "\\" + element.Name + ".csv";
                    var marketTable = element.GetTable(1000);
                    var marketTableData = marketTable.GetData();
                    PrepareMarketDataTable(marketTableData,elementPath);
                }
            }
        }

		public void PrepareMarketDataTable(IDictionary<string, object[]> tableData, string filePath)
        {
            var columns = typeof(MarketTable)
           .GetFields(BindingFlags.Public | BindingFlags.Static)
           .Select(f => f.GetValue(null)?.ToString()).ToArray();

            int dateIndex = Array.IndexOf(columns, MarketTable.lastUpdated);

            foreach (object[] rowValue in tableData.Values)
            {
                rowValue[dateIndex] = DateTime.FromOADate((double)rowValue[dateIndex]);
            }

            StoreTableData(columns, tableData, filePath);
        }

		public void StoreTableData(string[] columns, IDictionary<string, object[]> tableData, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine(string.Join(",", columns));
                writer.Flush();

                foreach (object[] rowValue in tableData.Values)
                {
                    writer.WriteLine(string.Join(",", rowValue));
                    writer.Flush();
                }
            }
        }
    }
}