using System;

namespace Jdownloader.Api.HttpClient
{
	public class JDownloaderHttpException : Exception
	{
		public JDownloaderHttpException(string message)
			: base(message)
		{ }
	}
}