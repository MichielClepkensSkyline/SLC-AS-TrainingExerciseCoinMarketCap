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
	using System.Collections.Generic;
	using System.IO;

	using CoinMarketCap_2.DataProcessors;
	using CoinMarketCap_2.Dtos;
	using CoinMarketCap_2.Processors;

	using Skyline.DataMiner.Automation;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string FilePathBase = "C:/Skyline DataMiner/Documents/";
		private const int LatestListingElementId = 14;
		private const int CategoriesElementId = 15;
		private const string GlobalMetricsCsvName = "GlobalMetrics";
		private const string CategoriesCsvName = "Categories";
		private const string LatestListingCsvName = "LatestListing";

		private readonly ElementTableConfigDto _categoriesTableConfig = new ElementTableConfigDto
		{
			AgentId = 161,
			ElementId = CategoriesElementId,
			TableId = 70,
			LastTableColumnId = 80,
			TableDateColumnIds = new List<int> { 79, 80 },
		};

		private readonly ElementTableConfigDto _latestListingTableConfig = new ElementTableConfigDto
		{
			AgentId = 161,
			ElementId = LatestListingElementId,
			TableId = 10,
			LastTableColumnId = 27,
			TableDateColumnIds = new List<int> { 27 },
		};

		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			var folderPath = GetFolderPath(engine);

			if (!ValidateFolderExists(engine, folderPath))
				return;

			CreateAndStartProcessors(engine, folderPath);
		}

		private static string GetFolderPath(IEngine engine) => $"{FilePathBase}{engine.GetScriptParam("folderName").Value}/";

		private static bool ValidateFolderExists(IEngine engine, string folderPath)
		{
			if (Directory.Exists(folderPath))
				return true;

			engine.Log($"Folder '{folderPath}' does not exist.");
			engine.ExitFail("Folder that you provided does not exist.");
			return false;
		}

		private void CreateAndStartProcessors(IEngine engine, string folderPath)
		{
			var globalMetricsProcessor = new GlobalMetricsProcessor(engine);
			var categoriesProcessor = new GeneralTablesProcessor(engine, _categoriesTableConfig);
			var latestListingProcessor = new GeneralTablesProcessor(engine, _latestListingTableConfig);

			globalMetricsProcessor.HandleExtractAndPrepareData(Path.Combine(folderPath, GlobalMetricsCsvName));
			categoriesProcessor.HandleExtractAndPrepareTableData(Path.Combine(folderPath, CategoriesCsvName));
			latestListingProcessor.HandleExtractAndPrepareTableData(Path.Combine(folderPath, LatestListingCsvName));
		}
	}
}