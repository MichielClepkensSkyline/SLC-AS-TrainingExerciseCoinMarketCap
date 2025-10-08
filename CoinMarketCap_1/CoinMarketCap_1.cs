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
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private static IDms dms;
		private List<CoinMarketCapElement> coinMarketCapElements = new List<CoinMarketCapElement>();
		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			if (!InitializeDMS(engine))
			{
				engine.ExitFail("Initialization failed");
			}

			// Get all elements with the CoinMarketCap protocol
			List<IDmsElement> elements = (List<IDmsElement>)dms.GetElements();
			foreach (IDmsElement element in elements)
			{
				if (element.Protocol.Name == "Exercise HTTP CoinMarketCap Sofian")
				{
					InitializeElement(engine, element);
				}
			}

			foreach (CoinMarketCapElement element in coinMarketCapElements)
			{
				engine.Log(element.GetName());
			}
			// Get the table with cryptocurrencies information in it
			coinMarketCapElement.GetLastListings();

			// IDms thisDms = engine.GetDms();
		}

		private bool InitializeDMS(IEngine engine)
		{
			try
			{
				dms = engine.GetDms();
			}
			catch (Exception e)
			{
				engine.Log("Initializing DMS failed: " + e);
				return false;
			}

			return true;
		}

		private bool InitializeElement(IEngine engine, IDmsElement coinMarketCapElement)
		{
			try
			{
				coinMarketCapElements.Add(new CoinMarketCapElement(engine, coinMarketCapElement));
			}
			catch (Exception e)
			{
				engine.Log("Initializing CoinMarketCap element failed: " + e);
				return false;
			}

			return true;
		}
	}
}