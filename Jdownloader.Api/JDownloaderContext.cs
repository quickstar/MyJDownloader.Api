using System;
using System.Collections.Generic;

using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;

namespace Jdownloader.Api
{
	public class JDownloaderContext
	{
		private readonly IJDownloaderHttpClient _jdownloaderClient;

		public JDownloaderContext(LoginDto loginDto, IJDownloaderHttpClient jdownloaderClient)
		{
			DeviceEncryptionToken = loginDto.DeviceEncryptionToken;
			ServerEncryptionToken = loginDto.ServerEncryptionToken;
			SessionToken = loginDto.SessionToken;
			RegainToken = loginDto.RegainToken;
			_jdownloaderClient = jdownloaderClient;
		}

		public byte[] DeviceEncryptionToken { get; set; }

		public string RegainToken { get; set; }

		public byte[] ServerEncryptionToken { get; set; }

		public string SessionToken { get; set; }

		public DevicesDto GetDevices()
		{
			const string route = "/my/listdevices";
			var queryParams = new Dictionary<string, string> { { "sessiontoken", SessionToken } };
			var result = _jdownloaderClient.Get<DevicesDto>(route, queryParams, ServerEncryptionToken);

			return result;
		}

		public JDownloaderApi SetDevice(DeviceDto device)
		{
			return new JDownloaderApi(this, device, _jdownloaderClient);
		}
	}
}