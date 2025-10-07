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
	using Skyline.DataMiner.Net.Helper;

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

			List<IDmsElement> elements = dms.GetElements().Where(p => p.Protocol.Name == protocolName).Where(x => x.State == ElementState.Active).ToList();

			//engine.Log($"DBG Tajana| Protocol name: {protocolName}.");

			for (int i = 0; i < elements.Count(); i++)
			{
				var elementShow = elements.ElementAt(i);
				//engine.Log($"DBG Tajana| Element {i + 1}: Name = {elementShow.Name}, ID = {elementShow.Id}.");
			}
		}
	}
}