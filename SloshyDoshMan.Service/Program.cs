using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.IO;

namespace SloshyDoshMan.Service
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				var configuration = SetupConfiguration(args);

				SetupLogging(configuration);
				StartWebHost(configuration);
			}
			catch (Exception exception)
			{
				Log.Fatal(exception, "Service terminated unexpectedly");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IConfiguration SetupConfiguration(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(ExecutableFolderPath)
				.AddJsonFile("service-settings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
				.AddCommandLine(args)
				.Build();

			Settings = new Settings(configuration);
			return configuration;
		}

		public static void SetupLogging(IConfiguration configuration)
		{
			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(configuration)
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.WriteTo.RollingFile("service-{Date}.log")
				.CreateLogger();
		}

		private static void StartWebHost(IConfiguration configuration)
		{
			WebHost = new WebHostBuilder()
				.UseKestrel()
				.UseConfiguration(configuration)
				.UseStartup<Startup>()
				.UseSerilog()
				.UseUrls($"http://*:{Settings.WebPort}")
				.Build();

			WebHost.Run();
		}

		public static ISettings Settings { get; private set; }
		public static IWebHost WebHost { get; private set; }

		public static string ExecutablePath { get; } = Process.GetCurrentProcess().MainModule.FileName;
		public static string ExecutableFolderPath { get; } = Path.GetDirectoryName(ExecutablePath);
	}
}
