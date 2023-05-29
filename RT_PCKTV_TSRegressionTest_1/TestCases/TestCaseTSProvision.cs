namespace RT_PCKTV_TSRegressionTest_1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Library.Tests.TestCases;
	using Newtonsoft.Json;
	using QAPortalAPI.Models.ReportingModels;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using static RT_PCKTV_TSRegressionTest_1.TSGenericClasses;

	public class TestCaseTSProvision : ITestCase
	{
		public TestCaseTSProvision(string name)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentNullException("name");
			}

			Name = name;
		}

		private enum ProvisionIndex
		{
			Result = 9,
			InstanceId = 11,
		}

		public string Name { get; set; }

		public TestCaseReport TestCaseReport { get; private set; }

		public PerformanceTestCaseReport PerformanceTestCaseReport { get; private set; }

		public void Execute(IEngine engine)
		{
			var tsElement = "Touchstream - VL";
			var dms = engine.GetDms();
			var idmsElement = dms.GetElement(tsElement);

			var assetId = String.Empty;
			var instanceId = Guid.NewGuid().ToString();
			var eventId = "wicuTest";
			var eventLabel = "RegressionTest";
			var eventName = "WICU Skyline Regression Test";
			var templateName = "Peacock Aff_01";
			var yospaceHls = "-1";
			var yospaceMpd = "-1";
			var endTime = DateTime.Now.ToOADate();

			Touchstream touchstream = new Touchstream
			{
				Action = (int)ConfigurationAction.Provision,
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
				EventStartDate = DateTime.Now.ToOADate(),
				EventEndDate = DateTime.FromOADate(endTime).AddHours(12).ToOADate(),
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
				TestCaseReport = TestCaseReport.GetSuccessTestCase(Name);
			}
			else
			{
				TestCaseReport = TestCaseReport.GetFailTestCase(Name, "Failed example");
			}
		}
	}
}
