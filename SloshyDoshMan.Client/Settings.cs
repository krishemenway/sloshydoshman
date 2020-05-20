using Microsoft.Extensions.Configuration;

namespace SloshyDoshMan.Client
{
	public interface ISettings
	{
		string KF2AdminHost { get; }
		int KF2AdminPort { get; }

		string KF2AdminUserName { get; }
		string KF2AdminPassword { get; }

		double RefreshIntervalInMilliseconds { get; }

		bool EnableAdvertisement { get; }
		string AdvertisementMessage { get; }

		string SloshyDoshManServiceHost { get; }
		int SloshyDoshManServicePort { get; }
	}

	public class Settings : ISettings
	{
		public Settings(IConfiguration configuration = null)
		{
			_configuration = configuration ?? Program.Configuration;
		}

		public string KF2AdminHost => _configuration.GetValue<string>("KF2AdminHost");
		public int KF2AdminPort => _configuration.GetValue<int>("KF2AdminPort");

		public string KF2AdminUserName => _configuration.GetValue<string>("KF2AdminUserName");
		public string KF2AdminPassword => _configuration.GetValue<string>("KF2AdminPassword");

		public double RefreshIntervalInMilliseconds => _configuration.GetValue<double>("RefreshIntervalInMilliseconds");

		public bool EnableAdvertisement => _configuration.GetValue<bool>("EnableAdvertisement");
		public string AdvertisementMessage => _configuration.GetValue<string>("AdvertisementMessage");

		public string SloshyDoshManServiceHost => _configuration.GetValue<string>("SloshyDoshManServiceHost");
		public int SloshyDoshManServicePort => _configuration.GetValue<int>("SloshyDoshManServicePort");

		private IConfiguration _configuration;
	}
}
