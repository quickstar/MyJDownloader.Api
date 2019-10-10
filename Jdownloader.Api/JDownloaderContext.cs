using System;
using System.Collections.Generic;

using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;

namespace Jdownloader.Api
{
	public class JDownloaderContext
	{
		private readonly IJDownloaderHttpClient _httpClient;

		public JDownloaderContext(LoginDto loginDto, IJDownloaderHttpClient httpClient)
		{
			DeviceEncryptionToken = loginDto.DeviceEncryptionToken;
			ServerEncryptionToken = loginDto.ServerEncryptionToken;
			SessionToken = loginDto.SessionToken;
			RegainToken = loginDto.RegainToken;
			_httpClient = httpClient;
		}

		public byte[] DeviceEncryptionToken { get; set; }

		public string RegainToken { get; set; }

		public byte[] ServerEncryptionToken { get; set; }

		public string SessionToken { get; set; }

		public DevicesDto GetDevices()
		{
			const string route = "/my/listdevices";
			var queryParams = new Dictionary<string, string> { { "sessiontoken", SessionToken } };
			var result = _httpClient.Get<DevicesDto>(route, queryParams, ServerEncryptionToken);

			return result;
		}

		public object SetDevice(DeviceItemDto device)
		{
			throw new NotImplementedException();
		}
	}
}