using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SloshyDoshMan.Service
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(ExecutablePath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			WebHost = new WebHostBuilder()
				.UseKestrel()
				.UseConfiguration(Configuration)
				.UseStartup<Startup>()
				.UseUrls($"http://*:{WebPort}")
				.Build();

			WebHost.Run();
		}

		public static int WebPort => Configuration.GetValue<int>("WebPort");

		public static IConfigurationRoot Configuration { get; private set; }
		public static IWebHost WebHost { get; private set; }

		public static string ExecutablePath => Directory.GetCurrentDirectory();
	}
}
