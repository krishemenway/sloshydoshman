using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;

namespace SloshyDoshMan.Client
{
	public class Settings
	{
		public Settings()
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(ExecutablePath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();
		}

		public string KF2AdminHost => Configuration.GetValue<string>("KF2AdminHost");
		public int KF2AdminPort => Configuration.GetValue<int>("KF2AdminPort");

		public string KF2AdminUserName => Configuration.GetValue<string>("KF2AdminUserName");
		public string KF2AdminPassword => Configuration.GetValue<string>("KF2AdminPassword");

		public double RefreshIntervalInMilliseconds => Configuration.GetValue<double>("RefreshIntervalInMilliseconds");

		public bool EnableAdvertisement => Configuration.GetValue<bool>("EnableAdvertisement");
		public string AdvertisementMessage => Configuration.GetValue<string>("AdvertisementMessage");

		public string SloshyDoshManServiceHost => Configuration.GetValue<string>("SloshyDoshManServiceHost"); 
		public int SloshyDoshManServicePort => Configuration.GetValue<int>("SloshyDoshManServicePort");

		public string ExecutablePath => Directory.GetCurrentDirectory();

		[JsonIgnore]
		public IConfiguration Configuration { get; }
	}
}
