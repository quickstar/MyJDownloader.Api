using Newtonsoft.Json;

namespace Jdownloader.Api.Models
{
	public class LoginDto : BaseDto
	{
		[JsonIgnore]
		public byte[] DeviceEncryptionToken;

		[JsonIgnore]
		public byte[] ServerEncryptionToken;

		[JsonProperty(PropertyName = "regaintoken")]
		public string RegainToken { get; set; }

		[JsonProperty(PropertyName = "sessiontoken")]
		public string SessionToken { get; set; }
	}
}