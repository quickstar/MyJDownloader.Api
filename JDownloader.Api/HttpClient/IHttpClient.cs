using System;

namespace Jdownloader.Api.HttpClient
{
	public interface IHttpClient
	{
		string Get(Uri uri);

		string Post(Uri uri);
	}
}