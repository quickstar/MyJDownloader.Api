using System;

using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;

namespace Jdownloader.Api.Namespaces
{
	public class DownloadController
	{
		private readonly JDownloaderContext _context;
		private readonly DeviceDto _device;
		private readonly IJDownloaderHttpClient _jdClient;

		public DownloadController(JDownloaderContext context, DeviceDto device, IJDownloaderHttpClient jdClient)
		{
			_context = context;
			_device = device;
			_jdClient = jdClient;
		}

		/// <summary>
		/// Gets the current state of the device
		/// </summary>
		/// <returns>The current state of the device.</returns>
		public string GetCurrentState()
		{
			var response = _jdClient.Post<DefaultReturnDto<string>>("/downloadcontroller/getCurrentState", _device, _context.SessionToken, _context.DeviceEncryptionToken);
			if (response != null)
			{
				return response.Data;
			}

			return "UNKOWN_STATE";
		}
	}
}