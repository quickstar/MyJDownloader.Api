using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;
using Jdownloader.Api.Namespaces;

namespace Jdownloader.Api
{
	public class JDownloaderApi
	{
		public JDownloaderApi(JDownloaderContext context, DeviceDto device, IJDownloaderHttpClient jdownloaderClient)
		{
			DownloadsV2 = new DownloadsV2(context, device, jdownloaderClient);
			Jd = new Jd(context, device, jdownloaderClient);
		}

		public DownloadsV2 DownloadsV2 { get; }

		public Jd Jd { get; }
	}
}