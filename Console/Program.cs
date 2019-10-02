using System;

using JDownloader.Api;

namespace JDownloader.Cli
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			var username = args[0];
			var password = args[1];

			var jdownloader = new JdownloaderClient();
			var loginDto = jdownloader.Connect(username, password);

			var listDevices = jdownloader.ListDevices(loginDto);
			foreach (var device in listDevices.List)
			{
				Console.WriteLine($"Device: {device.Name} ({device.Id})");
				Console.WriteLine($"Status: {device.Status}");
				Console.WriteLine($"Type: {device.Type}");
			}

			jdownloader.Disconnect(loginDto);

			listDevices = jdownloader.ListDevices(loginDto);

			Console.ReadKey();
		}
	}
}