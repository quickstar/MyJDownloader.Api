using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

using Jdownloader.Api.Crypto;
using Jdownloader.Api.Models;

using Newtonsoft.Json;

namespace Jdownloader.Api.HttpClient
{
	public class JdownloaderHttpClient : IHttpClient
	{
		private const string ApiUrl = "https://api.jdownloader.org";
		private const string Appkey = "my.jdownloader.api.wrapper";
		private const string DeviceApiSelector = "device";
		private const string ServerApiSelector = "server";

		private readonly CryptoUtils _cryptoUtils;

		public JdownloaderHttpClient(CryptoUtils cryptoUtils)
		{
			_cryptoUtils = cryptoUtils;
		}

		public T Get<T>(string url)
		{
			throw new NotImplementedException();
		}

		public T Post<T>(string url, object body)
		{
			throw new NotImplementedException();
		}

		public LoginDto Connect(string email, string password)
		{
			// Creating the query for the connection request
			// URL-Encoding
			//	Make sure all route parameters are correctly urlencoded.
			const string route = "/my/connect";
			var queryParams = new Dictionary<string, string> { { "email", email }, { "appkey", Appkey } };

			// Calculating the server login secret.
			// This is used only once to obtain the session token.
			var serverLoginSecret = _cryptoUtils.CreateServerLoginSecret(email, password, ServerApiSelector);

			// Execute the request
			var loginDto = ExecuteRequest<LoginDto>(route, queryParams, serverLoginSecret);

			var deviceLoginSecret = _cryptoUtils.CreateServerLoginSecret(email, password, DeviceApiSelector);

			// Enhance LoginDto with server- and device encryption tokens which in all
			// subsequent requests will be used to sign and decrypt the requests
			loginDto.ServerEncryptionToken = _cryptoUtils.CreateEncryptionToken(serverLoginSecret, loginDto.SessionToken);
			loginDto.DeviceEncryptionToken = _cryptoUtils.CreateEncryptionToken(deviceLoginSecret, loginDto.SessionToken);

			return loginDto;
		}

		/// <summary>
		/// Disconnects the your client from the api.
		/// </summary>
		/// <returns>True if successful</returns>
		public bool Disconnect(LoginDto login)
		{
			const string route = "/my/disconnect";
			var queryParams = new Dictionary<string, string> { { "sessiontoken", login.SessionToken } };

			var result = ExecuteRequest<BaseDto>(route, queryParams, login.ServerEncryptionToken);
			return result != null;
		}

		private T ExecuteRequest<T>(string route, IDictionary<string, string> queryParams, byte[] secret) where T : BaseDto
		{
			// TODO: queryParams need to be UrlEncoded properly
			// The RequestID is required in almost every request.
			//    It's a number that has to increase from one call to another.
			//    You can either use a millisecond precise timestamp, or a self incrementing number.
			//    The API will return the RequestID in the response.
			//    You should validate the response to make sure the answer is valid.
			var requestId = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
			queryParams.Add("rid", requestId.ToString(CultureInfo.InvariantCulture));

			// All Url parameters except the signature has to be propperly UrlEncoded.
			UrlEncodeQueryParams(queryParams);

			var fullUrl = BuildFullUrl(route, queryParams);
			var signature = _cryptoUtils.CalculateSignature(fullUrl.Uri.PathAndQuery, secret);
			queryParams.Add("signature", signature);

			fullUrl = BuildFullUrl(route, queryParams);

			var request = (HttpWebRequest)WebRequest.Create(fullUrl.Uri);
			// request.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
			try
			{
				var response = (HttpWebResponse)request.GetResponse();

				if (response.StatusCode != HttpStatusCode.OK)
				{
					response.Close();
					return default;
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
					result = _cryptoUtils.Decrypt(result, secret);

					var baseDto = Materialize<T>(result);
					if (baseDto.RequestId != requestId)
					{
						throw new WebException("Ups", WebExceptionStatus.ReceiveFailure);
					}

					return baseDto;
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
					}
				}
			}

			return null;
		}

		private static UriBuilder BuildFullUrl(string route, IDictionary<string, string> queryParams)
		{
			var queryString = string.Join("&", queryParams.Select(k => $"{k.Key}={k.Value}"));
			var fullUrl = new UriBuilder(new Uri(ApiUrl))
			{
				Path = route,
				Query = queryString
			};
			return fullUrl;
		}

		private void UrlEncodeQueryParams(IDictionary<string, string> queryParams)
		{
			foreach (var pair in queryParams.ToArray())
			{
				queryParams[pair.Key] = Uri.EscapeDataString(pair.Value);
			}
		}

		public DevicesDto ListDevices(LoginDto login)
		{
			const string route = "/my/listdevices";
			var queryParams = new Dictionary<string, string> { { "sessiontoken", login.SessionToken } };

			var result = ExecuteRequest<DevicesDto>(route, queryParams, login.ServerEncryptionToken);

			return result;
		}

		private T Materialize<T>(string rawjson)
		{
			return (T)JsonConvert.DeserializeObject(rawjson, typeof(T));
		}

		private void PrepareRequest()
		{ }
	}
}