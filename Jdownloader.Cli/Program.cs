using System;
using System.Linq;

using Jdownloader.Api;
using Jdownloader.Api.Crypto;
using Jdownloader.Api.HttpClient;
using Jdownloader.Api.Models;

namespace Jdownloader.Cli
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			var username = args[0];
			var password = args[1];

			var auth = new JDownloaderCredentials { Username = username, Password = password };

			var jdContext = new JDownloaderFactory().Create(auth);
			DevicesDto availableDevices = jdContext.GetDevices();

			foreach (var device in availableDevices.List)
			{
				Console.WriteLine($"Device: {device.Name} ({device.Id})");
				Console.WriteLine($"Type: {device.Type}");
			}

			var deviceApi = jdContext.SetDevice(availableDevices.List.First());
			var coreVersion = deviceApi.Jd.CoreRevision();
			var packages = deviceApi.DownloadsV2.QueryPackages(new LinkQueryObject());
			Console.WriteLine($"Version: {coreVersion}");
			Console.WriteLine($"Packages: {string.Join(Environment.NewLine + "- ", packages.Select(p => p.Name))}");

			Console.ReadKey();
		}
	}
}