using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
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
			SetupLogging();
			SetupConfiguration(args);
			ShowSettings();
			RegisterServer();
			StartMonitoring();
		}

		public static void SetupConfiguration(string[] args)
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
				.AddCommandLine(args)
				.Build();
		}

		public static void SetupLogging()
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.WriteTo.RollingFile("app-{Date}.log")
				.CreateLogger();
		}

		private static void RegisterServer()
		{
			try
			{
				new SloshyDoshManService().RegisterServer(new RegisterServerRequest { KF2ServerIP = Settings.KF2AdminHost });
			}
			catch (Exception e)
			{
				Log.Error(e, $"Failure during server registration: {e.Message}; {e.StackTrace}");
			}
		}

		private static void ShowSettings()
		{
			Log.Information("Using settings: {@Settings}", Settings);
			Log.Information($"Change your settings by adding EnvironmentVariables or modifying the appsettings.json or using command line parameters");
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

		public static readonly Settings Settings = new Settings();
		public static Guid? ServerId { get; set; }
		private static readonly IKillingFloor2AdminMonitor Monitor = new KillingFloor2AdminMonitor();

		public static IConfiguration Configuration { get; private set; }

		private static ManualResetEvent QuitEvent = new ManualResetEvent(false);
	}
}
