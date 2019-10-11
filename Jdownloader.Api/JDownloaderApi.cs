using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;
using Jdownloader.Api.Namespaces;

namespace Jdownloader.Api
{
	public class JDownloaderApi
	{
		public JDownloaderApi(JDownloaderContext context, DeviceDto device, IJDownloaderHttpClient jdownloaderClient)
		{
			Jd = new Jd(context, device, jdownloaderClient);
		}

		public Jd Jd { get; }
	}
}