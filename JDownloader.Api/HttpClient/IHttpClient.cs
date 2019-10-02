using System;

namespace JDownloader.Api.HttpClient
{
	public interface IHttpClient
	{
		T Get<T>(string url);

		T Post<T>(string url, object body);
	}
}