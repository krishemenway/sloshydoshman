
using Newtonsoft.Json;
using System;
using System.IO;

namespace SloshyDoshMan.Client
{
	public class Settings
	{
		static Settings()
		{
			if (File.Exists(SettingsJsonPath))
			{
				using (var file = File.OpenText(SettingsJsonPath))
				{
					Instance = JsonConvert.DeserializeObject<Settings>(file.ReadToEnd());
				}
			}
			else
			{
				Console.WriteLine($"Could not find settings.json file! Looked here: {SettingsJsonPath}");
				Instance = new Settings();
			}
		}
		
		public string KF2AdminHost { get; set; } = "localhost";
		public int KF2AdminPort { get; set; } = 3005;

		public string KF2AdminUserName { get; set; } = "admin";
		public string KF2AdminPassword { get; set; } = "123";
		
		public double RefreshIntervalInMilliseconds { get; set; } = 20000;

		public bool EnableAdvertisement { get; set; } = true;
		public string AdvertisementMessage { get; set; } = "Check out your stats for this server @ sloshydoshman.com";

		public static Settings Instance { get; set; }

		private static string SettingsJsonPath = Path.Combine(Program.ExecutingDirectory, "settings.json");
	}


}
