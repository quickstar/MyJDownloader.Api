using Jdownloader.Api.Models;

namespace Jdownloader.Api.HttpClient
{
	public interface IJdownloaderHttpClient : IHttpClient
	{
		LoginDto Connect(string email, string password);

		bool Disconnect(LoginDto login);

		DevicesDto ListDevices(LoginDto login);
	}
}