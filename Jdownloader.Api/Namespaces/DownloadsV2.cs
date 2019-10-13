using System;
using System.Collections.Generic;

using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;

namespace Jdownloader.Api.Namespaces
{
	public class DownloadsV2
	{
		private readonly JDownloaderContext _context;
		private readonly DeviceDto _device;
		private readonly IJDownloaderHttpClient _jdClient;

		public DownloadsV2(JDownloaderContext context, DeviceDto device, IJDownloaderHttpClient jdClient)
		{
			_context = context;
			_device = device;
			_jdClient = jdClient;
		}

		/// <summary>
		/// Gets all packages that are currently in the download list.
		/// </summary>
		/// <param name="linkQuery">An object which allows you to filter the return object.</param>
		/// <returns>An enumerable of the FilePackageDto which contains infos about the packages.</returns>
		public IEnumerable<FilePackageDto> QueryPackages(LinkQueryDto linkQuery)
		{
			var response = _jdClient.Post<DefaultReturnDto<IEnumerable<FilePackageDto>>>("/downloadsV2/queryPackages", _device, linkQuery, _context.SessionToken, _context.DeviceEncryptionToken);
			return response?.Data;
		}
	}
}