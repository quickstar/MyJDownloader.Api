using Newtonsoft.Json;

namespace JDownloader.Api.Models
{
	public class BaseDto
	{
		[JsonProperty(PropertyName = "rid")]
		public long RequestId { get; set; }
	}
}