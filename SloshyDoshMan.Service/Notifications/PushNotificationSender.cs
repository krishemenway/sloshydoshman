using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net.Http;
using System.Text;

namespace SloshyDoshMan.Service.Notifications
{
	public interface IPushNotificationSender
	{
		void NotifyAll(string typeName, string title, string content);
	}

	public class PushNotificationDetails
	{
		public string Title { get; set; }
		public string TypeName { get; set; }
		public int AlertNumber { get; set; }
		public string Content { get; set; }
	}

	public class PushNotificationSender : IPushNotificationSender
	{
		public PushNotificationSender(HttpClient httpClient = null)
		{
			_httpClient = httpClient ?? new HttpClient();
		}

		public void NotifyAll(string typeName, string title, string content)
		{
			var uri = new Uri($"http://{PushServiceHost}/internal/api/notifications/send");

			try
			{
				var details = new PushNotificationDetails
					{
						TypeName = typeName,
						Title = title,
						Content = content,
					};

				var requestContent = new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json");
				_httpClient.PostAsync(uri, requestContent).Wait();
			}
			catch (Exception exception)
			{
				Log.Error($"Failed making request to {uri}", exception);
			}
		}

		private string PushServiceHost => Program.Configuration.GetValue<string>("PushServiceHost");

		private readonly HttpClient _httpClient;
	}

	public class Notification
	{
		public string title { get; set; }
		public string body { get; set; }
		public string type { get; set; }
		public int typeid { get; set; }
		public int alertnumber { get; set; }
	}
}
