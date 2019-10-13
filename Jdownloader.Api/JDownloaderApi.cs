using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;
using Jdownloader.Api.Namespaces;

namespace Jdownloader.Api
{
	public class JDownloaderApi
	{
		public JDownloaderApi(JDownloaderContext context, DeviceDto device, IJDownloaderHttpClient jdownloaderClient)
		{
			DownloadController = new DownloadController(context, device, jdownloaderClient);
			DownloadsV2 = new DownloadsV2(context, device, jdownloaderClient);
			Jd = new Jd(context, device, jdownloaderClient);
			LinkgrabberV2 = new LinkgrabberV2(context, device, jdownloaderClient);
		}

		public DownloadController DownloadController { get; }

		public DownloadsV2 DownloadsV2 { get; }

		public Jd Jd { get; }

		public LinkgrabberV2 LinkgrabberV2 { get; }
	}
}