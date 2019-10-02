using Newtonsoft.Json;

namespace JDownloader.Api.Models
{
	public class LoginDto
	{
		[JsonProperty(PropertyName = "regaintoken")]
		public string RegainToken { get; set; }

		[JsonProperty(PropertyName = "rid")]
		public long RequestId { get; set; }

		[JsonProperty(PropertyName = "sessiontoken")]
		public string SessionToken { get; set; }
	}
}