using System;

using Newtonsoft.Json;

namespace Jdownloader.Api.Models
{
	public class CallActionObject : BaseDto
	{
		[JsonProperty(PropertyName = "ApiVer", Order = 4)]
		public int ApiVer { get; set; }

		[JsonProperty(PropertyName = "params", Order = 2)]
		public object Params { get; set; }

		[JsonProperty(PropertyName = "rid", Order = 3)]
		public override long RequestId { get; set; }

		[JsonProperty(PropertyName = "url", Order = 1)]
		public string Url { get; set; }
	}
}