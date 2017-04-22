using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace SloshyDoshMan.Client
{
	public class Program
	{
		static void Main()
		{
			Console.CancelKeyPress += delegate {
				_quitFlag = true;
			};

			PrintSplash();
			var monitor = new KillingFloor2AdminMonitor();
			monitor.StartMonitoring();

			while (!_quitFlag)
			{
				Thread.Sleep(1);
			}

			monitor.StopMonitoring();
		}

		private static void PrintSplash()
		{
			Console.WriteLine("-- SloshyDoshMan KF2 Stats Scraper --");
			Console.WriteLine("- Update the settings.json to configure for your local server");
			Console.WriteLine("CTRL-C to quit");
		}
		
		public static string ExecutingDirectory
		{
			get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
		}


		static bool _quitFlag = false;
	}
}
