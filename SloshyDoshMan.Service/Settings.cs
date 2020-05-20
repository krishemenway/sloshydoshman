using Microsoft.Extensions.Configuration;
using System.IO;

namespace SloshyDoshMan.Service
{
	public interface ISettings
	{
		int WebPort { get; }

		string SloshyDoshManDatabaseHost { get; }
		int SloshyDoshManDatabasePort { get; }

		string SloshyDoshManDatabaseName { get; }

		string SloshyDoshManDatabaseUser { get; }
		string SloshyDoshManDatabasePassword { get; }

		string SloshyDoshManPushServiceHost { get; }
		bool EnablePushNotification { get; }

		string ServerRegistrationKey { get; }

		string FilePathInExecutableFolder(string fileName);
	}

	public class Settings : ISettings
	{
		public Settings(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public int WebPort => _configuration.GetValue<int>("WebPort");

		public string SloshyDoshManDatabaseHost => _configuration.GetValue<string>("SloshyDoshManDatabaseHost");
		public int SloshyDoshManDatabasePort => _configuration.GetValue<int>("SloshyDoshManDatabasePort");

		public string SloshyDoshManDatabaseName => _configuration.GetValue<string>("SloshyDoshManDatabaseName");
		
		public string SloshyDoshManDatabaseUser => _configuration.GetValue<string>("SloshyDoshManDatabaseUser");
		public string SloshyDoshManDatabasePassword => _configuration.GetValue<string>("SloshyDoshManDatabasePassword");

		public string SloshyDoshManPushServiceHost => _configuration.GetValue<string>("SloshyDoshManPushServiceHost");
		public bool EnablePushNotification => _configuration.GetValue<bool>("EnablePushNotification");

		public string ServerRegistrationKey => _configuration.GetValue<string>("ServerRegistrationKey");

		public string FilePathInExecutableFolder(string fileName) => Path.Combine(Program.ExecutableFolderPath, fileName);

		private IConfiguration _configuration;
	}
}
