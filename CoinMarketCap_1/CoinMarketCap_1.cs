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

02/06/2025	1.0.0.1		LPA, Skyline	Initial version
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
    using Skyline.DataMiner.Net.Helper;

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
                // Catch normal abort exceptions (engine.ExitFail or engine.ExitSuccess)
                throw; // Comment if it should be treated as a normal exit of the script.
            }
            catch (ScriptForceAbortException)
            {
                // Catch forced abort exceptions, caused via external maintenance messages.
                throw;
            }
            catch (ScriptTimeoutException)
            {
                // Catch timeout exceptions for when a script has been running for too long.
                throw;
            }
            catch (InteractiveUserDetachedException)
            {
                // Catch a user detaching from the interactive script by closing the window.
                // Only applicable for interactive scripts, can be removed for non-interactive scripts.
                throw;
            }
            catch (Exception e)
            {
                engine.ExitFail("Run|Something went wrong: " + e);
            }
        }

        private void RunSafe(IEngine engine)
        {
            const int tableID = 100;
            string folderPath = FolderPath(engine);
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                engine.ExitFail("Invalid folder path specified.");
                return;
            }

            var dms = engine.GetDms();
            if (dms == null)
            {
                engine.ExitFail("Could not retrieve DMS system.");
                return;
            }

            var httpCoinMarketCapElements = dms.GetElements().
                Where(element => element.Protocol.Name.
                Contains("Exercise HTTP CoinMarketCap")).ToArray();
            if (httpCoinMarketCapElements.IsNullOrEmpty())
            {
                engine.ExitFail($"Elements with protocol name \"Exercise HTTP CoinMarketCap\" doesn't exist.");
            }

            foreach (var element in httpCoinMarketCapElements)
            {
                string elementName = element.Name;
                var table = element.GetTable(tableID);
                if (table == null)
                {
                    engine.ExitFail($"Could not retrieve Table with ID: {tableID}");
                    return;
                }

                var tableData = table.GetData();

                if (element.State == ElementState.Active)
                {
                    string csvFilePath = Path.Combine(folderPath, elementName + ".csv");
                    WriteDataToCsv(tableData, csvFilePath);
                }
            }
        }

        private void WriteDataToCsv(IDictionary<string, object[]> tableData, string path)
        {
            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var writer = new StreamWriter(path, false))
                {
                    writer.WriteLine("ID;Name;Price;Date Added;Last Update;Symbol;Price Change in 24h");
                    foreach (var row in tableData)
                    {
                        object[] values = row.Value ?? new object[0];
                        List<string> fieldsToWrite = new List<string>();

                        for (int i = 0; i < values.Length; i++)
                        {
                            object field = values[i];
                            string processedValue;

                            if (i == 3)
                            {
                                processedValue = ConvertToDate(field);
                            }
                            else if (i == 4)
                            {
                                processedValue = ConvertToDateTime(field);
                            }
                            else if (i == 2 || i == 6)
                            {
                                processedValue = EscapeNumericField(field);
                            }
                            else
                            {
                                processedValue = EscapeCsvField(field);
                            }

                            fieldsToWrite.Add(processedValue);
                        }

                        writer.WriteLine(string.Join(";", fieldsToWrite));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to write CSV to path '{path}': {ex.Message}", ex);
            }
        }

        private string EscapeNumericField(object field)
        {
            if (field == null)
                return string.Empty;

            // For numeric fields, add a tab character to prevent Excel auto-formatting
            if (field is double || field is float || field is decimal)
            {
                return "\t" + field.ToString();
            }
            return "\t" + field.ToString();
        }

        private string EscapeCsvField(object field)
        {
            if (field == null)
                return string.Empty;

            string str = field.ToString();
            if (str.Contains(",") || str.Contains("\"") || str.Contains("\n"))
            {
                str = str.Replace("\"", "\"\"");
                return $"\"{str}\"";
            }

            return str;
        }

        private string ConvertToDate(object field)
        {
            if (field == null)
                return string.Empty;

            if (double.TryParse(field.ToString(), out double dateValue))
            {
                // Convert Excel numeric date to Date
                DateTime date = DateTime.FromOADate(dateValue);
                return "\t" + date.ToString("yyyy-MM-dd"); // Add tab to prevent Excel auto-formatting
            }

            return "\t" + field.ToString();
        }

        private string ConvertToDateTime(object field)
        {
            if (field == null) return "";

            if (double.TryParse(field.ToString(), out double dateValue))
            {
                // Convert Excel numeric date to DateTime
                DateTime date = DateTime.FromOADate(dateValue);
                return "\t" + date.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return "\t" + field.ToString();
        }

        private string FolderPath(IEngine engine)
        {
            const int folderNameScriptParameterID = 10;
            string folderName = engine.GetScriptParam(folderNameScriptParameterID).Value;
            return $"C:\\Skyline DataMiner\\Documents\\{folderName}";
        }
    }
}