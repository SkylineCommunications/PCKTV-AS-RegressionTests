/*
****************************************************************************
*  Copyright (c) 2023,  Skyline Communications NV  All Rights Reserved.    *
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

dd/mm/2023	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

namespace RT_PCKTV_TSRegressionTest_Update_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using Newtonsoft.Json;
	using QAPortalAPI.Models.ReportingModels;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using static RT_PCKTV_TSRegressionTest_1.TSGenericClasses;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		private const string TestName = "RRT_PCKTV_TSRegressionTest_Provision";
		private const string TestDescription = "Regression Test to validate Touchstream provision.";

		private enum ProvisionIndex
		{
			Result = 9,
			InstanceId = 11,
		}

		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			TestReport testReport = new TestReport(
				new TestInfo(TestName, "Origin", new List<int>(), TestDescription),
				new TestSystemInfo("10.3.1", "PCK Lab"));

			var tsElement = "Touchstream - VL";
			var dms = engine.GetDms();
			var idmsElement = dms.GetElement(tsElement);

			var assetId = String.Empty;
			var instanceId = "2ed5baf3-448d-4103-9f67-8b04889b1762";
			var eventId = "wicuTest";
			var eventLabel = "RegressionTest";
			var eventName = "WICU Skyline Regression Test";
			var templateName = "Peacock Aff_01";
			var yospaceHls = "-1";
			var yospaceMpd = "-1";
			var endTime = DateTime.Now.ToOADate();

			Touchstream touchstream = new Touchstream
			{
				Action = (int)ConfigurationAction.Update,
				AssetId = assetId.Equals("-1") ? String.Empty : assetId,
				BookingId = instanceId,
				ConfigurationType = (int)StreamType.Regular,
				EventId = eventId,
				EventLabel = eventLabel,
				EventName = eventName,
				RowId = null,
				TemplateName = templateName,
				YoSpaceStreamIdHls = yospaceHls,
				YoSpaceStreamIdMpd = yospaceMpd,
				EventStartDate = DateTime.Now.AddMinutes(10).ToOADate(),
				EventEndDate = DateTime.FromOADate(endTime).AddHours(12).ToOADate(),
				DynamicGroup = "Linear_5",
				ReducedTemplate = false,
			};

			var jsonToSend = JsonConvert.SerializeObject(touchstream);

			var element = engine.FindElement(tsElement);
			element.SetParameter(20000, jsonToSend);

			bool CheckProvisionResult()
			{
				try
				{
					var provisionTable = idmsElement.GetTable(6400); // Dynamic Streams Provision table
					var tableRows = provisionTable.GetRows();

					foreach (var row in tableRows)
					{
						if (Convert.ToString(row[(int)ProvisionIndex.InstanceId]).Equals(instanceId) &&
							(Convert.ToString(row[(int)ProvisionIndex.Result]).Equals("Completed") || Convert.ToString(row[(int)ProvisionIndex.Result]).Equals("Completed with Errors")))
						{
							return true;
						}
					}

					return false;
				}
				catch (Exception ex)
				{
					engine.Log("Exception thrown while checking completed TS event: " + ex);
					throw;
				}
			}

			if (Retry(CheckProvisionResult, new TimeSpan(0, 3, 0)))
			{
				testReport.TryAddTestCase(TestCaseReport.GetSuccessTestCase(TestName));
				engine.GenerateInformation("Test successfully!!");
			}
			else
			{
				testReport.TryAddTestCase(TestCaseReport.GetFailTestCase(TestName, "Failed Touchstream provision Test"));
				engine.GenerateInformation("Failed Touchstream provision Test");
			}
		}
	}
}