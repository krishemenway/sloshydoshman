using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using SloshyDoshMan.Shared;
using System;
using System.Net.Http;
using System.Text;

namespace SloshyDoshMan.Service.Notifications
{
	public interface IPushNotificationSender
	{
		Result NotifyAll(string typeName, string title, string content);
	}

	public class PushNotificationSender : IPushNotificationSender
	{
		public PushNotificationSender(
			HttpClient httpClient = null,
			IConfiguration configuration = null)
		{
			_httpClient = httpClient ?? new HttpClient();
			_configuration = configuration ?? Program.Configuration;
		}

		public Result NotifyAll(string typeName, string title, string content)
		{
			if (!PushNotificationEnabled)
			{
				return Result.Successful;
			}

			try
			{
				var details = new PushNotificationDetails
					{
						TypeName = typeName,
						Title = title,
						Content = content,
					};

				var requestContent = new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json");
				var response = _httpClient.PostAsync(SendPushNotificationUri, requestContent).Result;
				return JsonConvert.DeserializeObject<Result>(response.Content.ReadAsStringAsync().Result);
			}
			catch (Exception exception)
			{
				Log.Error($"Failed making push notification request to {SendPushNotificationUri}. ${exception.Message}", exception);
				return Result.Failure($"Failed making push notification request to ({SendPushNotificationUri}): {exception.Message}");
			}
		}

		private string SendPushNotificationUri => $"http://{PushServiceHost}/internal/api/notifications/send";
		private string PushServiceHost => _configuration.GetValue<string>("SloshyDoshManPushServiceHost");
		private bool PushNotificationEnabled => _configuration.GetValue<bool>("EnablePushNotification");

		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		public class PushNotificationDetails
		{
			public string Title { get; set; }
			public string TypeName { get; set; }
			public int AlertNumber { get; set; }
			public string Content { get; set; }
		}
	}
}
