using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Jdownloader.Api.Crypto
{
	public class CryptoUtils
	{
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

		private static RijndaelManaged GetRijndaelManaged(byte[] ivKey)
		{
			if (ivKey == null)
			{
				throw new ArgumentNullException("The ivKey is null. Please check your login informations. If it's still null the server may has disconnected you.");
			}

			var iv = ivKey.Take(16).ToArray();
			var key = ivKey.Skip(16).Take(16).ToArray();
			var rj = new RijndaelManaged
			{
				BlockSize = 128,
				Mode = CipherMode.CBC,
				IV = iv,
				Key = key
			};
			return rj;
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
		public string CalculateSignature(string query, byte[] ivKey)
		{
			if (ivKey == null)
			{
				throw new Exception("The ivKey is null. Please check your login informations. If it's still null the server may has disconnected you.");
			}

			var dataBytes = Encoding.UTF8.GetBytes(query);
			byte[] hash;
			using (var hmacsha256 = new HMACSHA256(ivKey))
			{
				hmacsha256.ComputeHash(dataBytes);
				hash = hmacsha256.Hash;
			}

			var binaryString = hash.Aggregate("", (current, t) => current + t.ToString("X2"));
			return binaryString.ToLower();
		}

		public byte[] CreateEncryptionToken(byte[] loginSecret, string sessionToken)
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
		public byte[] CreateServerLoginSecret(string mail, string password, string apiIdentifier)
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
		public string Decrypt(string cypherText, byte[] ivKey)
		{
			var rj = GetRijndaelManaged(ivKey);

			try
			{
				var cypher = Convert.FromBase64String(cypherText);
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

		public string Encrypt(string text, byte[] ivKey)
		{
			var rj = GetRijndaelManaged(ivKey);

			ICryptoTransform encryptor = rj.CreateEncryptor();
			using (var memoryStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					using (var streamWriter = new StreamWriter(cryptoStream))
					{
						streamWriter.Write(text);
					}

					byte[] cypherData = memoryStream.ToArray();
					return Convert.ToBase64String(cypherData);
				}
			}
		}
	}
}