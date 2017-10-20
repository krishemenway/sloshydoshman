using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace SloshyDoshMan.Service.Notifications
{
	public interface IPushNotificationSender
	{
		void NotifyAll(PushNotificationDetails pushNotificationDetails);
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

		public void NotifyAll(PushNotificationDetails details)
		{
			var content = new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json");
			var response = _httpClient.PostAsync("http://localhost:8105/internal/api/notifications/send", content).Result;
		}

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
