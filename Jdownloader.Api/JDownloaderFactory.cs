using Jdownloader.Api.Crypto;
using Jdownloader.Api.HttpClient;

namespace Jdownloader.Api
{
	public class JDownloaderFactory
	{
		public JDownloaderContext Create(JDownloaderCredentials credentials)
		{
			var jdownloaderClient = new JDownloaderHttpClient(new CryptoUtils(), new WebRequestClient());
			var loginDto = jdownloaderClient.Connect(credentials.Username, credentials.Password);
			return new JDownloaderContext(loginDto, jdownloaderClient);
		}
	}
}