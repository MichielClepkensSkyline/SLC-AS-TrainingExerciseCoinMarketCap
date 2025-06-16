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

02/06/2025	1.0.0.1		DanijelBr, Skyline	Initial version
****************************************************************************
*/

namespace CoinMarketCap_1
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
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
            catch (DirectoryNotFoundException ex)
            {
                engine.GenerateInformation($"{ex}");
                engine.Log($"Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                engine.Log($"Run|Something went wrong: {ex}");
            }
        }

        public void RunSafe(IEngine engine)
		{
            var protocolName = "Exercise HTTP CoinMarketCap";
            const int marketTableID = 1000;
            const int cryptoListingTableID = 2000;
            const int folderPathScriptParameterId = 2;

            IDms dms = engine.GetDms();
            var elements = dms.GetElements().Where(x => x.Protocol.Name == protocolName);
            elements = elements.Where(x => x.State == ElementState.Active);
            engine.Log($"Preparing to export {elements.Count().ToString()} elements");
            string folderPath = engine.GetScriptParam(folderPathScriptParameterId).Value;
            if (!string.IsNullOrEmpty(folderPath))
            {
                if (elements != null && elements.Any())
                {
                    foreach (var element in elements)
                    {
                        if (element.State == ElementState.Active)
                        {
                            string elementPath = folderPath + "\\" + element.Name + ".csv";

                            var marketTable = element.GetTable(marketTableID);
                            var marketTableData = marketTable?.GetData();
                            var csvMarketData = PrepareMarketDataTable(marketTableData);
                            ExportTableData(csvMarketData, elementPath, false);

                            var cryptoListingTable = element.GetTable(cryptoListingTableID);
                            var cryptoListingTableData = cryptoListingTable?.GetData();
                            var csvCryptoListing = PrepareCryptoListingTable(cryptoListingTableData);
                            ExportTableData(csvCryptoListing, elementPath, true);
                        }
                    }
                }
            }
            else
            {
                engine.Log($"Folder path cannot be empty. Please provide a valid path.");
            }
        }

        public string PrepareMarketDataTable(IDictionary<string, object[]> tableData)
        {
            var columns = typeof(MarketTable)
           .GetFields(BindingFlags.Public | BindingFlags.Static)
           .Select(folder => folder.GetValue(null)?.ToString()).ToArray();

            int dateIndex = Array.IndexOf(columns, MarketTable.lastUpdated);

            if (tableData != null && tableData.Any())
            {
                foreach (object[] rowValue in tableData.Values)
                {
                    rowValue[dateIndex] = DateTime.FromOADate((double)rowValue[dateIndex]);
                }
            }

            return BuildCsvContent(columns, tableData);
        }

        public string PrepareCryptoListingTable(IDictionary<string, object[]> tableData)
        {
            var columns = typeof(CryptoListingTable)
           .GetFields(BindingFlags.Public | BindingFlags.Static)
           .Select(folder => folder.GetValue(null)?.ToString()).ToArray();

            return BuildCsvContent(columns, tableData);
        }

        public void ExportTableData(string data, string filePath, bool append)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (Directory.Exists(directory))
            {
                using (var writer = new StreamWriter(filePath, append))
                {
                    if (append)
                    {
                        writer.WriteLine();
                        writer.Flush();
                    }

                    writer.Write(data);
                    writer.Flush();
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"The folder path '{directory}' does not exist.");
            }
        }

        public string BuildCsvContent(string[] columns, IDictionary<string, object[]> tableData)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(string.Join(",", columns));

            if (tableData != null && tableData.Any())
            {
                foreach (object[] rowValue in tableData.Values)
                {
                    stringBuilder.AppendLine(string.Join(",", rowValue));
                }
            }

            return stringBuilder.ToString();
        }
    }
}