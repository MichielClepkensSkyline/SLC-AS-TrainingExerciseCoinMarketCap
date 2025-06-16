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
    using System.Text;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;

    /// <summary>
    /// Represents a DataMiner Automation script.
    /// </summary>
    public class Script
    {
        /// <summary>
        /// The Script entry point.
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
            catch (Exception ex)
            {
                engine.ExitFail("Run|Something went wrong: " + ex);
            }
        }

        public void RunSafe(IEngine engine)
        {
            engine.SetFlag(RunTimeFlags.NoKeyCaching);
            engine.Timeout = TimeSpan.FromHours(10);

            // Fixed params
            const int tableId = 1000;
            const string protocolName = "Exercise HTTP CoinMarketCap";
            const int lastUpdateColumnId = 3; // Last Update column id
            const int folderNameScriptParameterId = 2;

            var columns = new List<string>
            {
               "Rank", "Name", "Symbol", "Last Update", "Market Cap", "Circulating Supply", "Price", "1h%", "24h%", "7d%",
            };

            string folderName = engine.GetScriptParam(folderNameScriptParameterId).Value;
            IDms dms = engine.GetDms();

            var elements = dms.GetElements().
                Where(protocol => protocol.Protocol.Name == protocolName).
                Where(element=>element.State == ElementState.Active).ToList(); // Get all active elements

            if (elements.Count > 0)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    var element = elements[i];
                    var path = CreatePath(folderName, element.Name);
                    var tableData = element.GetTable(tableId).GetData();
                    WriteTableDataToCsvfile(tableData, path, columns, lastUpdateColumnId);
                }
            }
            else
            {
                engine.ExitFail("Run|No elements found using protocol '" + protocolName + "'");
            }
        }

        public string CreatePath(string folderName, string elementName)
        {
            if (string.IsNullOrWhiteSpace(elementName))
            {
                throw new Exception($"Element name {elementName} is empty!");
            }

            folderName = string.IsNullOrWhiteSpace(folderName) ? "CSV files" : folderName;
            return $"C:\\Skyline DataMiner\\Documents\\{folderName}\\{elementName}.csv";
        }

        public void WriteTableDataToCsvfile(IDictionary<string, object[]> tableData, string path, List<string> columns, int lastUpdateColumnId)
        {
            var directory = Path.GetDirectoryName(path);
            if (Directory.Exists(directory))
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Join(",", columns));

                if (tableData.Count > 0)
                {
                    foreach (object[] row in tableData.Values)
                    {
                        if (row[lastUpdateColumnId] is double oaDate)
                        {
                            row[lastUpdateColumnId] = DateTime.FromOADate(oaDate);
                        }

                        stringBuilder.AppendLine(string.Join(",", row));
                    }

                    File.WriteAllText(path, stringBuilder.ToString());
                }
                else
                {
                    throw new Exception("Table is empty!");
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"The folder path '{directory}' does not exist.");
            }
        }
    }
}