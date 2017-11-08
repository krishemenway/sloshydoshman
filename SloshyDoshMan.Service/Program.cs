﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace SloshyDoshMan.Service
{
	public class Program
	{
		public static void Main(string[] args)
		{
			SetupLogging();

			try
			{
				SetupConfiguration();
				StartWebHost();
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

		private static void SetupConfiguration()
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();
		}

		private static void SetupLogging()
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.WriteTo.RollingFile("app-{Date}.log")
				.CreateLogger();
		}

		private static void StartWebHost()
		{
			WebHost = new WebHostBuilder()
				.UseKestrel()
				.UseConfiguration(Configuration)
				.UseStartup<Startup>()
				.UseSerilog()
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
