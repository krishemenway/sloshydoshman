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
			ISettings settings = null)
		{
			_httpClient = httpClient ?? new HttpClient();
			_settings = settings ?? Program.Settings;
		}

		public Result NotifyAll(string typeName, string title, string content)
		{
			if (!_settings.EnablePushNotification)
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

		private string SendPushNotificationUri => $"http://{_settings.SloshyDoshManPushServiceHost}/internal/api/notifications/send";

		private readonly HttpClient _httpClient;
		private readonly ISettings _settings;

		public class PushNotificationDetails
		{
			public string Title { get; set; }
			public string TypeName { get; set; }
			public int AlertNumber { get; set; }
			public string Content { get; set; }
		}
	}
}
