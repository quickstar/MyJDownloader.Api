using Newtonsoft.Json;

namespace Jdownloader.Api.Models
{
	public class BaseDto
	{
		[JsonProperty(PropertyName = "rid")]
		public long RequestId { get; set; }
	}
}