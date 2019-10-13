using System;

using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;

namespace Jdownloader.Api.Namespaces
{
	public class Jd
	{
		private readonly JDownloaderContext _context;
		private readonly DeviceDto _device;
		private readonly IJDownloaderHttpClient _jdClient;

		public Jd(JDownloaderContext context, DeviceDto device, IJDownloaderHttpClient jdClient)
		{
			_context = context;
			_device = device;
			_jdClient = jdClient;
		}

		public int CoreRevision()
		{
			var response = _jdClient.Post<DefaultReturnDto<int>>("/jd/getCoreRevision", _device, _context.SessionToken, _context.DeviceEncryptionToken);
			if (response == null)
			{
				return -1;
			}

			return response.Data;
		}

		/// <summary>
		/// Gets the version of the JDownloader client.
		/// </summary>
		/// <returns>The current version of the JDownloader client.</returns>
		public long Version()
		{
			var response = _jdClient.Post<DefaultReturnDto<long>>("/jd/version", _device, _context.SessionToken, _context.DeviceEncryptionToken);
			if (response == null)
			{
				return -1;
			}

			return response.Data;
		}
	}
}