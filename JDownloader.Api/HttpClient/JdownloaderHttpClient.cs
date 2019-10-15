using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Jdownloader.Api.Crypto;
using Jdownloader.Api.Exceptions;
using Jdownloader.Api.Models;

using Newtonsoft.Json;

namespace Jdownloader.Api.HttpClient
{
	public class JDownloaderHttpClient : IJDownloaderHttpClient
	{
		private const string ApiUrl = "http://api.jdownloader.org";
		private const string Appkey = "my.jdownloader.api.wrapper";
		private const string DeviceApiSelector = "device";
		private const string ServerApiSelector = "server";

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

		private readonly CryptoUtils _cryptoUtils;
		private readonly IHttpClient _httpClient;

		public JDownloaderHttpClient(CryptoUtils cryptoUtils, IHttpClient httpClient)
		{
			_cryptoUtils = cryptoUtils;
			_httpClient = httpClient;
		}

		public LoginDto Connect(string email, string password)
		{
			// Creating the query for the connection request
			const string route = "/my/connect";
			var queryParams = new Dictionary<string, string> { { "email", email }, { "appkey", Appkey } };

			// Calculating the server and device login secret.
			// This is used only once to obtain the session token.
			var serverLoginSecret = _cryptoUtils.CreateServerLoginSecret(email, password, ServerApiSelector);
			var deviceLoginSecret = _cryptoUtils.CreateServerLoginSecret(email, password, DeviceApiSelector);

			// Execute the request
			var loginDto = ExecuteRequest<LoginDto>(route, queryParams, null, true, serverLoginSecret);

			// Enhance LoginDto with server- and device encryption tokens which in all
			// subsequent requests will be used to sign and encrypt/decrypt the requests
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

			var result = ExecuteRequest<BaseDto>(route, queryParams, null, false, login.ServerEncryptionToken);
			return result != null;
		}

		public T Get<T>(string route, Dictionary<string, string> queryParams, byte[] key) where T : BaseDto
		{
			return ExecuteRequest<T>(route, queryParams, null, true, key);
		}

		public T Post<T>(string route, DeviceDto device, object data, string sessionToken, byte[] key, bool validateRequest = false) where T : BaseDto
		{
			string query = $"/t_{Uri.EscapeDataString(sessionToken)}_{Uri.EscapeDataString(device.Id)}{route}";
			CallActionDto body = new CallActionDto
			{
				ApiVer = 1,
				Params = data != null ? new [] { JsonConvert.SerializeObject(data) } : null,
				Url = route
			};

			var result = ExecuteRequest<T>(query, new Dictionary<string, string>(), body, validateRequest, key);

			return result;
		}

		public T Post<T>(string route, DeviceDto device, string sessionToken, byte[] key) where T : BaseDto
		{
			return Post<T>(route, device, null, sessionToken, key, false);
		}

		private T ExecuteRequest<T>(string route, IDictionary<string, string> queryParams, BaseDto body, bool validateRequest, byte[] secret) where T : BaseDto
		{
			// The RequestID is required in almost every request.
			//    It's a number that has to increase from one call to another.
			//    You can either use a millisecond precise timestamp, or a self incrementing number.
			//    The API will return the RequestID in the response.
			//    You should validate the response to make sure the answer is valid.
			var requestId = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

			// If we have a body the request id is sent via payload, otherwise it's a part of the query params
			if (body != null)
			{
				body.RequestId = requestId;
			}
			else
			{
				queryParams.Add("rid", requestId.ToString(CultureInfo.InvariantCulture));

				// All Url parameters except the signature has to be propperly UrlEncoded.
				UrlEncodeQueryParams(queryParams);
			}

			var fullUrl = BuildFullUrl(route, queryParams);
			if (queryParams.Any())
			{
				var signature = _cryptoUtils.CalculateSignature(fullUrl.Uri.PathAndQuery, secret);
				queryParams.Add("signature", signature);
				fullUrl = BuildFullUrl(route, queryParams);
			}

			string result;
			if (body != null)
			{
				string payload = Serialize(body);
				string cypherText = _cryptoUtils.Encrypt(payload, secret);
				try
				{
					result = _httpClient.Post(fullUrl.Uri, cypherText);
				}
				catch (JDownloaderHttpException e)
				{
					result = e.Message;
				}
			}
			else
			{
				result = _httpClient.Get(fullUrl.Uri);
			}

			result = _cryptoUtils.Decrypt(result, secret);

			var baseDto = Materialize<T>(result);
			if (validateRequest && baseDto.RequestId != requestId)
			{
				throw new InvalidRequestIdException("The received 'RequestId' differs from the 'RequestId' sent by the query.");
			}

			return baseDto;
		}

		private T Materialize<T>(string rawjson)
		{
			return (T)JsonConvert.DeserializeObject(rawjson, typeof(T));
		}

		private string Serialize(object body)
		{
			return JsonConvert.SerializeObject(body);
		}

		private void UrlEncodeQueryParams(IDictionary<string, string> queryParams)
		{
			foreach (var pair in queryParams.ToArray())
			{
				queryParams[pair.Key] = Uri.EscapeDataString(pair.Value);
			}
		}
	}
}
