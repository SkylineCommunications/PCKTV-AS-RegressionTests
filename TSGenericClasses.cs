namespace RT_PCKTV_TSRegressionTest_1
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using Newtonsoft.Json;

	public class TSGenericClasses
	{
		public enum ConfigurationAction // Action
		{
			Provision = 0,
			Deactivate = 1,
			Update = 4,
		}

		public enum StreamType // Configuration Type
		{
			Regular = 0,
			Adobe = 1,
		}

		public static bool Retry(Func<bool> func, TimeSpan timeout)
		{
			bool success = false;

			Stopwatch sw = new Stopwatch();
			sw.Start();

			do
			{
				success = func();
				if (!success)
				{
					Thread.Sleep(3000);
				}
			}
			while (!success && sw.Elapsed <= timeout);

			return success;
		}

		public class Touchstream
		{
			[JsonProperty("Action", NullValueHandling = NullValueHandling.Ignore)]
			public long? Action { get; set; }

			[JsonProperty("AssetId")]
			public object AssetId { get; set; }

			[JsonProperty("BookingId", NullValueHandling = NullValueHandling.Ignore)]
			public string BookingId { get; set; }

			[JsonProperty("ConfigurationType", NullValueHandling = NullValueHandling.Ignore)]
			public long? ConfigurationType { get; set; }

			[JsonProperty("EventId")]
			public object EventId { get; set; }

			[JsonProperty("EventLabel", NullValueHandling = NullValueHandling.Ignore)]
			public string EventLabel { get; set; }

			[JsonProperty("EventName", NullValueHandling = NullValueHandling.Ignore)]
			public string EventName { get; set; }

			[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
			public string Id { get; set; }

			[JsonProperty("Manifests", NullValueHandling = NullValueHandling.Ignore)]
			public List<MediaTailorManifest> Manifests { get; set; }

			[JsonProperty("RowId")]
			public object RowId { get; set; }

			[JsonProperty("TemplateName", NullValueHandling = NullValueHandling.Ignore)]
			public string TemplateName { get; set; }

			[JsonProperty("YoSpaceStreamIdHls", NullValueHandling = NullValueHandling.Ignore)]
			public string YoSpaceStreamIdHls { get; set; }

			[JsonProperty("YoSpaceStreamIdMpd", NullValueHandling = NullValueHandling.Ignore)]
			public string YoSpaceStreamIdMpd { get; set; }

			[JsonProperty("DynamicGroup", NullValueHandling = NullValueHandling.Ignore)]
			public string DynamicGroup { get; set; }

			[JsonProperty("ReducedTemplate", NullValueHandling = NullValueHandling.Ignore)]
			public bool ReducedTemplate { get; set; }

			[JsonProperty("EventStartDate", NullValueHandling = NullValueHandling.Ignore)]
			public double EventStartDate { get; set; }

			[JsonProperty("EventEndDate", NullValueHandling = NullValueHandling.Ignore)]
			public double EventEndDate { get; set; }

			[JsonProperty("ForceUpdate", NullValueHandling = NullValueHandling.Ignore)]
			public bool ForceUpdate { get; set; }
		}

		public class MediaTailorManifest
		{
			[JsonProperty("url")]
			public string Url { get; set; }

			[JsonProperty("product")]
			public string Product { get; set; }

			[JsonProperty("cdn")]
			public string Cdn { get; set; }

			[JsonProperty("format")]
			public string Format { get; set; }
		}
	}
}
