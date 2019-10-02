using System;

namespace Jdownloader.Api.HttpClient
{
	public interface IHttpClient
	{
		T Get<T>(string url);

		T Post<T>(string url, object body);
	}
}