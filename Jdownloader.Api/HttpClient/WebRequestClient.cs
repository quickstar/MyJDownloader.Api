using System;
using System.IO;
using System.Net;
using System.Text;

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

		public string Post(Uri uri, string payload)
		{
			return ExecWebRequest(uri, WebRequestMethods.Http.Post, r =>
			{
				r.ContentType = "application/aesjson-jd; charset=utf-8";
				var data = Encoding.UTF8.GetBytes(payload);
				r.ContentLength = data.Length;
				using (var stream = r.GetRequestStream())
				{
					stream.Write(data, 0, data.Length);
				}
			});
		}

		private string ExecWebRequest(Uri uri, string method, Action<HttpWebRequest> requestAction = null)
		{
			var request = (HttpWebRequest)WebRequest.Create(uri);
			request.Method = method;

			requestAction?.Invoke(request);

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
					using (var streamReader = new StreamReader(responseStream))
					{
						result = streamReader.ReadToEnd();
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
					using (var streamReader = new StreamReader(respsone))
					{
						string errorMsg = streamReader.ReadToEnd();
						throw new JDownloaderHttpException(errorMsg);
					}
				}
			}

			return null;
		}
	}
}