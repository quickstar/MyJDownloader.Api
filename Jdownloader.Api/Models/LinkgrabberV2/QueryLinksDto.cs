using Newtonsoft.Json;

namespace Jdownloader.Api.Models.LinkgrabberV2
{
	public class QueryLinksDto
	{
		[JsonProperty(PropertyName = "availability")]
		public bool Availability { get; set; }

		[JsonProperty(PropertyName = "maxResults")]
		public int MaxResults { get; set; }

		[JsonProperty(PropertyName = "url")]
		public bool Url { get; set; }

	}
}
