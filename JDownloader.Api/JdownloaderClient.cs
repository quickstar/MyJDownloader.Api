using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

using JDownloader.Api.Models;

using Newtonsoft.Json;

namespace JDownloader.Api
{
	public class JdownloaderClient
	{
		private const string ApiUrl = "https://api.jdownloader.org";
		private const string Appkey = "my.jdownloader.api.wrapper";
		private const string DeviceApiSelector = "device";
		private const string ServerApiSelector = "server";

		private static string ExecuteRequest(string url)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			// request.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
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
				if (exception.Response != null)
				{
					var respsone = exception.Response.GetResponseStream();
					if (respsone != null)
					{
						var resp = new StreamReader(respsone).ReadToEnd();
					}
				}
			}

			return null;
		}

		private static byte[] GetByteArrayByHexString(string hexString)
		{
			hexString = hexString.Replace("-", "");
			byte[] ret = new byte[hexString.Length / 2];
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
			}

			return ret;
		}

		/// <summary>
		/// Create the Signature:
		/// 1. build the full queryString (incl. RequestID)
		/// 2. hmac the queryString. The used Key depends. Some calls use serverEncryptionToken,
		/// others have to ask the user for email and password, create the loginSecret and use the loginsecret as key.
		/// email needs to be lower case!
		/// 3. hexformat the result
		/// 4. append the signature to the queryString &signature=
		/// Example:
		/// queryString = "/my/connect?email=foo@bar.com&rid=1361982773157";
		/// queryString += "&signature=" + HmacSha256(utf8bytes(queryString), ServerEncryptionToken);
		/// </summary>
		private string CalculateSignature(string query, byte[] key)
		{
			if (key == null)
			{
				throw new Exception("The ivKey is null. Please check your login informations. If it's still null the server may has disconnected you.");
			}

			var dataBytes = Encoding.UTF8.GetBytes(query);
			byte[] hash;
			using (var hmacsha256 = new HMACSHA256(key))
			{
				hmacsha256.ComputeHash(dataBytes);
				hash = hmacsha256.Hash;
			}

			var binaryString = hash.Aggregate("", (current, t) => current + t.ToString("X2"));
			return binaryString.ToLower();
		}

		public LoginDto Connect(string email, string password)
		{
			// The RequestID is required in almost every request.
			//    It's a number that has to increase from one call to another.
			//    You can either use a millisecond precise timestamp, or a self incrementing number.
			//    The API will return the RequestID in the response.
			//    You should validate the response to make sure the answer is valid.
			var requestId = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

			// Creating the query for the connection request
			// URL-Encoding
			//	Make sure all url parameters are correctly urlencoded.
			var query = "/my/connect?" +
						$"email={Uri.EscapeDataString(email)}" +
						$"&appkey={Uri.EscapeDataString(Appkey)}" +
						$"&rid={requestId}";

			// Calculating the server login secrete.
			// This is used only once to optain the session token.
			var serverLoginSecret = CreateServerLoginSecret(email, password, ServerApiSelector);

			// Since we do not yet have a session token
			// we use the serverLoginSecret instead to sign the query
			var signature = CalculateSignature(query, serverLoginSecret);
			query += "&signature=" + signature;

			var url = ApiUrl + query;

			var result = ExecuteRequest(url);
			result = Decrypt(result, serverLoginSecret);
			var loginDto = Materialize<LoginDto>(result);

			if (loginDto.RequestId != requestId)
			{
				throw new WebException("Ups", WebExceptionStatus.ReceiveFailure);
			}

			loginDto.ServerEncryptionToken = CreateEncryptionToken(serverLoginSecret, loginDto.SessionToken);

			// Calculating the device login secrete.
			var deviceLoginSecret = CreateServerLoginSecret(email, password, DeviceApiSelector);
			loginDto.ServerEncryptionToken = CreateEncryptionToken(serverLoginSecret, loginDto.SessionToken);
			loginDto.DeviceEncryptionToken = CreateEncryptionToken(deviceLoginSecret, loginDto.SessionToken);

			return loginDto;
		}

		private byte[] CreateEncryptionToken(byte[] loginSecret, string sessionToken)
		{
			byte[] newToken = GetByteArrayByHexString(sessionToken);
			var newHash = new byte[loginSecret.Length + newToken.Length];
			loginSecret.CopyTo(newHash, 0);
			newToken.CopyTo(newHash, 32);

			using (var sha256Managed = new SHA256Managed())
			{
				sha256Managed.ComputeHash(newHash);
				return sha256Managed.Hash;
			}
		}

		/// <summary>
		/// There are two different main routes available.
		/// - Server API
		/// - Device API
		/// The server API handles device registrations, account creation, account modification, password resets and so on.
		/// Furthermore it handles the handshake between the request client and the JdownloaderClient client.
		/// The device API offers the routes you need to get data from the JdownloaderClient client.
		/// </summary>
		private byte[] CreateServerLoginSecret(string mail, string password, string apiIdentifier)
		{
			var stringtemplate = mail.ToLower() + password + apiIdentifier;
			using (var hashing = new SHA256Managed())
			{
				return hashing.ComputeHash(Encoding.UTF8.GetBytes(stringtemplate));
			}
		}

		/// <summary>
		/// Response Encryption:
		/// aes128cbc(iv=serverEncryptionToken.firstHalf, key=serverEncryptionToken.secondHalf)
		/// </summary>
		private string Decrypt(string cypherText, byte[] ivKey)
		{
			if (ivKey == null)
			{
				throw new Exception("The ivKey is null. Please check your login informations. If it's still null the server may has disconnected you.");
			}

			var iv = ivKey.Take(16).ToArray();
			var key = ivKey.Skip(16).Take(16).ToArray();

			try
			{
				var cypher = Convert.FromBase64String(cypherText);
				var rj = new RijndaelManaged
				{
					BlockSize = 128,
					Mode = CipherMode.CBC,
					IV = iv,
					Key = key
				};

				string result;
				using (var ms = new MemoryStream(cypher))
				{
					using (var cs = new CryptoStream(ms, rj.CreateDecryptor(), CryptoStreamMode.Read))
					{
						using (var sr = new StreamReader(cs))
						{
							result = sr.ReadToEnd();
						}
					}
				}

				return result;
			}
			catch (Exception)
			{
				return cypherText;
			}
		}

		/// <summary>
		/// Disconnects the your client from the api.
		/// </summary>
		/// <returns>True if successful</returns>
		public bool Disconnect(LoginDto login)
		{
			// The RequestID is required in almost every request.
			//    It's a number that has to increase from one call to another.
			//    You can either use a millisecond precise timestamp, or a self incrementing number.
			//    The API will return the RequestID in the response.
			//    You should validate the response to make sure the answer is valid.
			var requestId = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

			string query = $"/my/disconnect?sessiontoken={Uri.EscapeDataString(login.SessionToken)}&rid={requestId}";
			var signature = CalculateSignature(query, login.ServerEncryptionToken);
			query += "&signature=" + signature;

			var url = ApiUrl + query;
			var result = ExecuteRequest(url);
			result = Decrypt(result, login.ServerEncryptionToken);
			var resultDto = Materialize<BaseDto>(result);

			if (resultDto.RequestId != requestId)
			{
				return false;
			}

			return true;
		}

		public DevicesDto ListDevices(LoginDto login)
		{
			// The RequestID is required in almost every request.
			//    It's a number that has to increase from one call to another.
			//    You can either use a millisecond precise timestamp, or a self incrementing number.
			//    The API will return the RequestID in the response.
			//    You should validate the response to make sure the answer is valid.
			var requestId = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

			string query = $"/my/listdevices?sessiontoken={Uri.EscapeDataString(login.SessionToken)}&rid={requestId}";
			var signature = CalculateSignature(query, login.ServerEncryptionToken);
			query += "&signature=" + signature;

			var url = ApiUrl + query;
			var result = ExecuteRequest(url);
			result = Decrypt(result, login.ServerEncryptionToken);
			var resultDto = Materialize<DevicesDto>(result);

			if (resultDto.RequestId != requestId)
			{
				throw new WebException("Ups", WebExceptionStatus.ReceiveFailure);
			}

			return resultDto;
		}

		private T Materialize<T>(string rawjson)
		{
			return (T)JsonConvert.DeserializeObject(rawjson, typeof(T));
		}
	}
}