using System;

using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;
using Jdownloader.Api.Namespaces;

namespace Jdownloader.Api
{
	public class JDownloader
	{
		private readonly DeviceItemDto _device;
		private readonly JDownloaderHttpClient _httpClient;
		private readonly LoginDto _loginDto;

		public JDownloader(DeviceItemDto device, JDownloaderHttpClient httpClient, LoginDto loginDto)
		{
			_device = device;
			_httpClient = httpClient;
			_loginDto = loginDto;
			Jd = new Jd(device, httpClient, loginDto);
		}

		public Jd Jd { get; }
	}
}