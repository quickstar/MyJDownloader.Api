using System;

using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;

namespace Jdownloader.Api.Namespaces
{
	public class Jd
	{
		private readonly DeviceItemDto _device;
		private readonly IJDownloaderHttpClient _httpClient;
		private readonly LoginDto _loginDto;

		public Jd(DeviceItemDto device, IJDownloaderHttpClient httpClient, LoginDto loginDto)
		{
			_device = device;
			_httpClient = httpClient;
			_loginDto = loginDto;
		}

		/// <summary>
		/// Gets the version of the JDownloader client.
		/// </summary>
		/// <returns>The current version of the JDownloader client.</returns>
		public long Version()
		{
			/*_httpClient.Post<BaseDto>(_device, "/jd/version", null, _loginDto);
			var response = ApiHandler.CallAction<DefaultReturnObject>(_device, "/jd/version", null, LoginObject, true);
			if (response == null)
			{
				return -1;
			}

			return (long)response.Data;*/
			return 0;
		}
	}
}