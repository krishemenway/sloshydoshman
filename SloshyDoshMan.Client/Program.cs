using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace SloshyDoshMan.Client
{
	public class Program
	{
		public static void Main(string[] args)
		{
			AddLogging();
			ShowSettings();
			StartMonitoring();
		}

		private static void AddLogging()
		{
			LoggerFactory.AddConsole();
		}

		private static void ShowSettings()
		{
			LoggerFactory.CreateLogger<Program>().LogInformation($"Loaded Settings: {JsonConvert.SerializeObject(Settings)}");
			LoggerFactory.CreateLogger<Program>().LogInformation($"Change your settings by adding EnvironmentVariables or modifying the appsettings.json");
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
		private static readonly IKillingFloor2AdminMonitor Monitor = new KillingFloor2AdminMonitor();

		private static ManualResetEvent QuitEvent = new ManualResetEvent(false);
	}
}
