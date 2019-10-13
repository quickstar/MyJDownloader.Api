using System;
using System.Collections.Generic;

using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;
using Jdownloader.Api.Models.LinkgrabberV2;

namespace Jdownloader.Api.Namespaces
{
	public class LinkgrabberV2
	{
		private readonly JDownloaderContext _context;
		private readonly DeviceDto _device;
		private readonly IJDownloaderHttpClient _jdClient;

		public LinkgrabberV2(JDownloaderContext context, DeviceDto device, IJDownloaderHttpClient jdClient)
		{
			_context = context;
			_device = device;
			_jdClient = jdClient;
		}

		/// <summary>
		/// Adds a download link to the given device.
		/// </summary>
		/// <param name="requestDto">
		/// Contains informations like the link itself or the priority.
		/// If you want to use multiple links sperate them with an ';' char.
		/// </param>
		public bool AddLinks(AddLinkRequestDto requestDto)
		{
			requestDto.Links = requestDto.Links.Replace(";", "\\r\\n");
			var response = _jdClient.Post<DefaultReturnDto<string[]>>("/linkgrabberv2/addLinks", _device, _context.SessionToken, _context.DeviceEncryptionToken);
			return response != null;
		}

		/// <summary>
		/// Gets the selection base of the download folder history.
		/// </summary>
		/// <returns>An array which contains the download folder history.</returns>
		public string[] GetDownloadFolderHistorySelectionBase()
		{
			var response = _jdClient.Post<DefaultReturnDto<string[]>>("/linkgrabberv2/getDownloadFolderHistorySelectionBase", _device, _context.SessionToken, _context.DeviceEncryptionToken);
			return response?.Data;
		}

		/// <summary>
		/// Checks if the JDownloader client is still collecting files from links.
		/// </summary>
		/// <returns>Returns true or false. Depending on if the client is still collecting files.</returns>
		public bool IsCollecting()
		{
			var response = _jdClient.Post<DefaultReturnDto<bool>>("/linkgrabberv2/isCollecting", _device, _context.SessionToken, _context.DeviceEncryptionToken);
			if (response == null)
			{
				return false;
			}

			return response.Data;
		}

		/// <summary>
		/// Moves one or multiple links/packages to the download list.
		/// </summary>
		/// <param name="linkIds">The ids of the links you want to move.</param>
		/// <param name="packageIds">The ids of the packages you want to move.</param>
		/// <returns>True if successfull.</returns>
		public bool MoveToDownloadlist(long[] linkIds, long[] packageIds)
		{
			var param = new[] { linkIds, packageIds };

			var response = _jdClient.Post<DefaultReturnDto<bool>>("/linkgrabberv2/moveToDownloadlist", _device, param, _context.SessionToken, _context.DeviceEncryptionToken);
			if (response == null)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Gets all links that are currently in the linkcollector.
		/// </summary>
		/// <param name="maxResults">Maximum number of return values.</param>
		/// <returns>An enumerable of all links that are currently in the linkcollector list.</returns>
		public IEnumerable<QueryLinksResponseDto> QueryLinks(int maxResults = -1)
		{
			QueryLinksDto queryLink = new QueryLinksDto
			{
				Availability = true,
				Url = true
			};
			if (maxResults > 0)
			{
				queryLink.MaxResults = maxResults;
			}

			var response = _jdClient.Post<DefaultReturnDto<IEnumerable<QueryLinksResponseDto>>>("/linkgrabberv2/queryLinks", _device, queryLink, _context.SessionToken, _context.DeviceEncryptionToken);
			return response?.Data;
		}

		/// <summary>
		/// Gets a list of available packages that are currently in the linkcollector.
		/// </summary>
		/// <param name="requestObject">The request object which contains properties to define the return properties.</param>
		/// <returns>An enumerable of all available packages.</returns>
		public IEnumerable<QueryPackagesResponseDto> QueryPackages(QueryPackagesRequestDto requestObject)
		{
			var response = _jdClient.Post<DefaultReturnDto<IEnumerable<QueryPackagesResponseDto>>>("/linkgrabberv2/queryPackages", _device, requestObject, _context.SessionToken, _context.DeviceEncryptionToken);
			return response?.Data;
		}

		/// <summary>
		/// Rename an existing package. After renaming the old packageId is no longer valid.
		/// </summary>
		/// <param name="packageId">The id of the package you want to rename.</param>
		/// <param name="newName">The name of the new package.</param>
		/// <returns>True if successfull.</returns>
		public bool RenamePackage(long packageId, string newName)
		{
			var param = new object[] { packageId, newName };
			var response = _jdClient.Post<DefaultReturnDto<bool>>("/linkgrabberv2/renamePackage", _device, param, _context.SessionToken, _context.DeviceEncryptionToken);

			var renameSuccess = string.IsNullOrEmpty(response?.Data.ToString());
			return renameSuccess;
		}
	}
}