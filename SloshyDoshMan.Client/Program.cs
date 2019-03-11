using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SloshyDoshMan.Shared;
using System;
using System.IO;
using System.Threading;

namespace SloshyDoshMan.Client
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
				.AddCommandLine(args)
				.Build();

			AddLogging();
			ShowSettings();
			RegisterServer();
			StartMonitoring();
		}

		private static void RegisterServer()
		{
			var serverName = new KillingFloor2AdminScraper().FindServerName();
			var request = new RegisterServerRequest { KF2ServerIP = Settings.KF2AdminHost, ServerName = serverName };

			new SloshyDoshManService().RegisterServer(request);
		}

		private static void AddLogging()
		{
			LoggerFactory.AddConsole();
		}

		private static void ShowSettings()
		{
			LoggerFactory.CreateLogger<Program>().LogInformation($"Using settings: {JsonConvert.SerializeObject(Settings)}");
			LoggerFactory.CreateLogger<Program>().LogInformation($"Change your settings by adding EnvironmentVariables or modifying the appsettings.json or using command line parameters");
		}

		private static void StartMonitoring()
		{
			Console.CancelKeyPress += (sender, eArgs) =>
			{
				QuitEvent.Set();
				eArgs.Cancel = true;
			};

			Monitor.StartMonitoring();
			QuitEvent.WaitOne();
			Monitor.StopMonitoring();
		}

		public static readonly LoggerFactory LoggerFactory = new LoggerFactory();
		public static readonly Settings Settings = new Settings();
		public static Guid? ServerId { get; set; }
		private static readonly IKillingFloor2AdminMonitor Monitor = new KillingFloor2AdminMonitor();

		public static IConfiguration Configuration { get; private set; }

		private static ManualResetEvent QuitEvent = new ManualResetEvent(false);
	}
}
