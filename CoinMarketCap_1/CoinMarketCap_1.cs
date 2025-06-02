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

02/06/2025	1.0.0.1		ABA, Skyline	Initial version
****************************************************************************
*/

namespace CoinMarketCap_1
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;

    /// <summary>
    /// Represents a DataMiner Automation script.
    /// </summary>
    public class Script
    {
        private IEngine engine;

        /// <summary>
        /// The Script entry point.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        public void Run(IEngine engine)
        {
            try
            {
                // Fixed params
                const int tableId = 1000;
                const string protocolName = "Exercise HTTP CoinMarketCap";

                string folderName = engine.GetScriptParam(2).Value;
                this.engine = engine;
                IDms dms = engine.GetDms();

                // Get all elements
                var elements = dms.GetElements().Where(x => x.Protocol.Name == protocolName).ToArray();
                string elementName1 = elements[3].Name;
                string elementName2 = elements[1].Name;
                string elementName3 = elements[2].Name;

                // engine.GenerateInformation("Element: " + elements[2].Name);

                // First element
                var element1 = dms.GetElement(elementName1);
                var table1 = element1.GetTable(tableId).GetData();
                var path1 = CreatePath(folderName, elementName1);

                // engine.GenerateInformation("Table1 count is: " + table1.Count);
                WriteTableDataToCsvfile(table1, path1);

                // Second element
                var element2 = dms.GetElement(elementName2);
                var table2 = element1.GetTable(tableId).GetData();
                var path2 = CreatePath(folderName, elementName2);

                // engine.GenerateInformation("Table2 count is: " + table2.Count);
                WriteTableDataToCsvfile(table2, path2);

                // Third element
                var element3 = dms.GetElement(elementName3);
                var table3 = element1.GetTable(tableId).GetData();
                var path3 = CreatePath(folderName, elementName3);

                // engine.GenerateInformation("Table3 count is: " + table3.Count);
                WriteTableDataToCsvfile(table3, path3);

                // Scheduler
                var dma = dms.GetAgent(Engine.SLNetRaw.ServerDetails.AgentID);
                var scheduler = dma.Scheduler;

                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now.AddDays(2);

                string activStartDay = startDate.ToString("yyyy - MM - dd");
                string activStopDay = endDate.ToString("yyyy - MM - dd");

                string startTime = startDate.ToString("HH: mm:ss");
                string endTime = endDate.ToString("HH: mm:ss");

                string taskType = "daily";
                string runInterval = "1"; // Run interval (x minutes(daily) / 1,…,31,101,102(monthly) / 1,3,5,7 (1=Monday, 7=Sunday)(weekly))

                string scriptName = "CoinMarketCap";
                string elemLinked = string.Empty;
                string paramLinked = string.Empty;
                string taskName = "GetRefreshData";
                string taskDescription = "Get refreshed data and store him in CSV files.";

                object[] data = new object[]
                {
                    new object []
                    {
                        new string []
                        {
                            taskName,
                            activStartDay,
                            activStopDay,
                            startTime,
                            taskType,
                            runInterval,
                            string.Empty,
                            taskDescription,
                            "TRUE",
                            endTime,
                            string.Empty,
                        },
                    },
                    new object[]
                       {
                        new string[]
                        {
                            "automation",
                            scriptName,
                            elemLinked,
                            paramLinked,
                            // elem2Linked,
                            "CHECKSETS:FALSE",
                            "DEFER: False",
                        },
                       },
                    new object[] {},
                };

                int taskId = scheduler.CreateTask(data);
                engine.GenerateInformation("Task id: " + taskId);

                engine.SetFlag(RunTimeFlags.NoKeyCaching);
                engine.Timeout = TimeSpan.FromHours(10);
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
            catch (Exception ex)
            {
                engine.ExitFail("Run|Something went wrong: " + ex);
            }
        }

        private void WriteTableDataToCsvfile(IDictionary<string, object[]> tableData, string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                int lastUpdateColumnId = 3;
                streamWriter.WriteLine(string.Join(",", "Rank", "Name", "Symbol", "Last Update", "Makret Cap", "Circulating Suppy", "Price", "1h%", "24h%", "7d%" ));
                streamWriter.Flush();

                foreach (object[] row in tableData.Values)
                {
                    row[lastUpdateColumnId] = DateTime.FromOADate((double)row[3]);
                    streamWriter.WriteLine(string.Join(",", row));
                    streamWriter.Flush();
                }
            }
        }

        private string CreatePath(string folderName, string elementName)
        {
            return "C:\\Skyline DataMiner\\Documents\\"+ folderName + "\\" + elementName + ".csv";
        }
    }
}