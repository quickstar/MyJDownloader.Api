using Newtonsoft.Json;

namespace Jdownloader.Api.Models
{
	public class LinkQueryDto
	{
		[JsonProperty(PropertyName = "addedDate")]
		public bool AddedDate { get; set; } = true;

		[JsonProperty(PropertyName = "bytesLoaded")]
		public bool BytesLoaded { get; set; } = true;

		[JsonProperty(PropertyName = "bytesTotal")]
		public bool BytesTotal { get; set; } = true;

		[JsonProperty(PropertyName = "comment")]
		public bool Comment { get; set; } = true;

		[JsonProperty(PropertyName = "enabled")]
		public bool Enabled { get; set; } = true;

		[JsonProperty(PropertyName = "eta")]
		public bool Eta { get; set; } = true;

		[JsonProperty(PropertyName = "extractionStatus")]
		public bool ExtractionStatus { get; set; } = true;

		[JsonProperty(PropertyName = "finished")]
		public bool Finished { get; set; } = true;

		[JsonProperty(PropertyName = "finishedDate")]
		public bool FinishedDate { get; set; } = true;

		[JsonProperty(PropertyName = "host")]
		public bool Host { get; set; } = true;

		[JsonProperty(PropertyName = "jobUUIDs")]
		public long[] JobUUIDs { get; set; }

		[JsonProperty(PropertyName = "maxResults")]
		public int MaxResults { get; set; } = 20;

		[JsonProperty(PropertyName = "packageUUIDs")]
		public long[] PackageUUIDs { get; set; }

		[JsonProperty(PropertyName = "password")]
		public bool Password { get; set; } = true;

		[JsonProperty(PropertyName = "priority")]
		public bool Priority { get; set; } = true;

		[JsonProperty(PropertyName = "running")]
		public bool Running { get; set; } = true;

		[JsonProperty(PropertyName = "skipped")]
		public bool Skipped { get; set; } = true;

		[JsonProperty(PropertyName = "speed")]
		public bool Speed { get; set; } = true;

		[JsonProperty(PropertyName = "startAt")]
		public int StartAt { get; set; }

		[JsonProperty(PropertyName = "status")]
		public bool Status { get; set; } = true;

		[JsonProperty(PropertyName = "url")]
		public bool Url { get; set; }
	}
}