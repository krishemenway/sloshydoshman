using Microsoft.Extensions.Configuration;

namespace SloshyDoshMan.Client
{
	public class Settings
	{
		public string KF2AdminHost => Program.Configuration.GetValue<string>("KF2AdminHost");
		public int KF2AdminPort => Program.Configuration.GetValue<int>("KF2AdminPort");

		public string KF2AdminUserName => Program.Configuration.GetValue<string>("KF2AdminUserName");
		public string KF2AdminPassword => Program.Configuration.GetValue<string>("KF2AdminPassword");

		public double RefreshIntervalInMilliseconds => Program.Configuration.GetValue<double>("RefreshIntervalInMilliseconds");

		public bool EnableAdvertisement => Program.Configuration.GetValue<bool>("EnableAdvertisement");
		public string AdvertisementMessage => Program.Configuration.GetValue<string>("AdvertisementMessage");

		public string SloshyDoshManServiceHost => Program.Configuration.GetValue<string>("SloshyDoshManServiceHost");
		public int SloshyDoshManServicePort => Program.Configuration.GetValue<int>("SloshyDoshManServicePort");
	}
}
