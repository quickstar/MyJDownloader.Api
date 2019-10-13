using Jdownloader.Api.Models.LinkgrabberV2;

using Newtonsoft.Json;

namespace Jdownloader.Api.Models.DownloadsV2
{
	public class FilePackageDto
	{
		[JsonProperty(PropertyName = "activeTask")]
		public string ActiveTask { get; set; }

		[JsonProperty(PropertyName = "bytesLoaded")]
		public long BytesLoaded { get; set; }

		[JsonProperty(PropertyName = "bytesTotal")]
		public long BytesTotal { get; set; }

		[JsonProperty(PropertyName = "childCount")]
		public int ChildCount { get; set; }

		[JsonProperty(PropertyName = "downloadPassword")]
		public string DownloadPassword { get; set; }

		[JsonProperty(PropertyName = "enabled")]
		public bool Enabled { get; set; }

		[JsonProperty(PropertyName = "eta")]
		public long Eta { get; set; }

		[JsonProperty(PropertyName = "finished")]
		public bool Finished { get; set; }

		[JsonProperty(PropertyName = "host")]
		public string Host { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "priority")]
		public PriorityType Priority { get; set; }

		[JsonProperty(PropertyName = "running")]
		public bool Running { get; set; }

		[JsonProperty(PropertyName = "saveTo")]
		public string SaveTo { get; set; }

		[JsonProperty(PropertyName = "speed")]
		public long Speed { get; set; }

		[JsonProperty(PropertyName = "status")]
		public string Status { get; set; }

		[JsonProperty(PropertyName = "statusIconKey")]
		public string StatusIconKey { get; set; }

		[JsonProperty(PropertyName = "uuid")]
		public long UUID { get; set; }
	}
}