using Newtonsoft.Json;

namespace Jdownloader.Api.Models
{
	public class BaseDto
	{
		[JsonProperty(PropertyName = "rid")]
		public virtual long RequestId { get; set; }
	}
}