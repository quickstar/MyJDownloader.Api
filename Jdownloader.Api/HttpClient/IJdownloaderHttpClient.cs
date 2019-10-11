using System.Collections.Generic;

using Jdownloader.Api.Models;

namespace Jdownloader.Api.HttpClient
{
	public interface IJDownloaderHttpClient
	{
		LoginDto Connect(string email, string password);

		bool Disconnect(LoginDto login);

		T Get<T>(string route, Dictionary<string, string> queryParams, byte[] key) where T : BaseDto;

		T Post<T>(string route, DeviceDto device, IDictionary<string, string> queryParams, bool valideRequest, string sessionToken, byte[] key) where T : BaseDto;

		T Post<T>(string route, DeviceDto device, string sessionToken, byte[] key) where T : BaseDto;
	}
}