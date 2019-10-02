namespace Console
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			var username = args[0];
			var password = args[1];

			var jdownloader = new JDownloader.Api.JDownloader();
			var loginDto = jdownloader.Connect(username, password);
		}
	}
}