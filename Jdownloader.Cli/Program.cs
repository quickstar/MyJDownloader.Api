using System;
using System.Linq;

using Jdownloader.Api.Crypto;
using Jdownloader.Api.HttpClient;

namespace Jdownloader.Cli
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			var username = args[0];
			var password = args[1];

			var jdownloaderHttpClient = new JdownloaderHttpClient(new CryptoUtils());
			var loginDto = jdownloaderHttpClient.Connect(username, password);

			var listDevices = jdownloaderHttpClient.ListDevices(loginDto);
			foreach (var device in listDevices.List)
			{
				Console.WriteLine($"Device: {device.Name} ({device.Id})");
				Console.WriteLine($"Type: {device.Type}");
			}

			var activeDevice = listDevices.List.First();

			var jdownloader = new Api.Jdownloader(activeDevice, jdownloaderHttpClient, loginDto);
			jdownloader.Jd.Version();

			Console.ReadKey();
		}
	}
}