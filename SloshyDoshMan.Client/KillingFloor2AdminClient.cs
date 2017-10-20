using SloshyDoshMan.Shared;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SloshyDoshMan.Client
{
	public interface IKillingFloor2AdminClient
	{
		string GetPlayersContent();
		string GetScoreboardContent();

		void SendMessage(string message);
	}

	public class KillingFloor2AdminClient : IKillingFloor2AdminClient
	{
		public void SendMessage(string message)
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthorizationHeader);
				
				var content = new StringContent($"message={HttpUtility.UrlEncode(message)}&teamsay=-1", Encoding.UTF8, "application/x-www-form-urlencoded");
				var requestTask = client
					.PostAsync(CreateKF2AdminServerPath("/ServerAdmin/current/chat+frame+data"), content)
					.ContinueWith(task =>
					{
						if (task.IsFaulted || task.IsCanceled)
						{
							throw new KF2ServerOfflineException();
						}

						return task.Result;
					});

				var response = requestTask.Result;

				if (response.StatusCode == HttpStatusCode.Unauthorized)
				{
					throw new InvalidAdminCrendentialsException();
				}

				response.Content.ReadAsStringAsync().Wait();
			}
		}

		public string GetPlayersContent()
		{
			return GetContentFromUri(CreateKF2AdminServerPath("/ServerAdmin/current/players"));
		}

		public string GetScoreboardContent()
		{
			return GetContentFromUri(CreateKF2AdminServerPath("/ServerAdmin/current/info"));
		}

		private string GetContentFromUri(Uri uri)
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthorizationHeader);

				var requestTask = client
					.GetAsync(uri)
					.ContinueWith(task =>
					{
						if (task.IsFaulted || task.IsCanceled)
						{
							throw new KF2ServerOfflineException();
						}

						return task.Result;
					});

				var response = requestTask.Result;

				if (response.StatusCode == HttpStatusCode.Unauthorized)
				{
					throw new InvalidAdminCrendentialsException();
				}

				return response.Content.ReadAsStringAsync().Result;
			}
		}

		private Uri CreateKF2AdminServerPath(string path)
		{
			return new Uri($"http://{Program.Settings.KF2AdminHost}:{Program.Settings.KF2AdminPort}{path}");
		}

		private string AuthorizationHeader => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Program.Settings.KF2AdminUserName}:{Program.Settings.KF2AdminPassword}"));
	}
}
