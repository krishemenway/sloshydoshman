using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using SloshyDoshMan.Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SloshyDoshMan.Client
{
	public class Program
	{
		public static void Main(string[] args)
		{
			SetupConfiguration(args);
			SetupLogging();
			ShowSettings();
			RegisterServer();
			StartMonitoring();
		}

		public static void SetupConfiguration(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
				.AddJsonFile("SloshyDoshMan.Client.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
				.AddCommandLine(args)
				.Build();

			Settings = new Settings(configuration);
		}

		public static void SetupLogging()
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.WriteTo.RollingFile("SloshyDoshMan.Client-{Date}.log")
				.CreateLogger();
		}

		private static void RegisterServer()
		{
			try
			{
				new SloshyDoshManService().RegisterServer(new RegisterServerRequest { KF2ServerIP = Settings.KF2AdminHost, RegistrationKey = Settings.ServerRegistrationKey });
			}
			catch (Exception exception)
			{
				Log.Error(exception, $"Failure during server registration: {exception.Message}; {exception.StackTrace}");
				throw exception;
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

		public static Settings Settings { get; private set; }
		public static Guid? ServerId { get; set; }
		private static readonly IKillingFloor2AdminMonitor Monitor = new KillingFloor2AdminMonitor();

		private static ManualResetEvent QuitEvent = new ManualResetEvent(false);
	}
}
