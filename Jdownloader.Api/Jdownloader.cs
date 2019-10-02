using System;

using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;
using Jdownloader.Api.Namespaces;

namespace Jdownloader.Api
{
	public class Jdownloader
	{
		private readonly DeviceItemDto _device;
		private readonly JdownloaderHttpClient _httpClient;
		private readonly LoginDto _loginDto;

		public Jdownloader(DeviceItemDto device, JdownloaderHttpClient httpClient, LoginDto loginDto)
		{
			_device = device;
			_httpClient = httpClient;
			_loginDto = loginDto;
			Jd = new Jd(device, httpClient, loginDto);
		}

		public Jd Jd { get; }
	}
}