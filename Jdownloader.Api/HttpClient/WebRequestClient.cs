using System;
using System.IO;
using System.Net;

namespace Jdownloader.Api.HttpClient
{
	/// <summary>
	/// HttpClient which can be used with .NET Framework 4.0.
	/// </summary>
	public class WebRequestClient : IHttpClient
	{
		public string Get(Uri uri)
		{
			return ExecWebRequest(uri, WebRequestMethods.Http.Get);
		}

		public string Post(Uri uri)
		{
			return ExecWebRequest(uri, WebRequestMethods.Http.Post, h => h.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8"));
		}

		private string ExecWebRequest(Uri uri, string method, Action<WebHeaderCollection> additionalHeaders = null)
		{
			var request = (HttpWebRequest)WebRequest.Create(uri);
			request.Method = method;

			additionalHeaders?.Invoke(request.Headers);

			try
			{
				var response = (HttpWebResponse)request.GetResponse();
				if (response.StatusCode != HttpStatusCode.OK)
				{
					response.Close();
					return null;
				}

				using (var responseStream = response.GetResponseStream())
				{
					if (responseStream == null)
					{
						return null;
					}

					string result = null;
					using (var myStreamReader = new StreamReader(responseStream))
					{
						result = myStreamReader.ReadToEnd();
					}

					response.Close();
					return result;
				}
			}
			catch (WebException exception)
			{
				var respsone = exception.Response?.GetResponseStream();
				if (respsone != null)
				{
					using (var resp = new StreamReader(respsone))
					{
						string errorMsg = resp.ReadToEnd();
						throw new JDownloaderHttpException(errorMsg);
					}
				}
			}

			return null;
		}
	}
}