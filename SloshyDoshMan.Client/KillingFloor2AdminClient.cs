using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using SloshyDoshMan.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SloshyDoshMan.Client
{
	public interface IKillingFloor2AdminClient
	{
		IDocument GetPlayersContent();
		IDocument GetScoreboardContent();

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

		public IDocument GetPlayersContent()
		{
			return BrowsingContext.New().OpenAsync(GetContentFromUri(CreateKF2AdminServerPath("/ServerAdmin/current/players"))).Result;
		}

		public IDocument GetScoreboardContent()
		{
			return BrowsingContext.New().OpenAsync(GetContentFromUri(CreateKF2AdminServerPath("/ServerAdmin/current/info"))).Result;
		}

		private IResponse GetContentFromUri(Uri uri)
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

				return new AdminClientResponse
				{
					StatusCode = response.StatusCode,
					Address = new Url(response.RequestMessage.RequestUri.AbsoluteUri),
					Content = response.Content.ReadAsStreamAsync().Result,
					Headers = new Dictionary<string, string>(),
				};
			}
		}

		private Uri CreateKF2AdminServerPath(string path)
		{
			return new Uri($"http://{Program.Settings.KF2AdminHost}:{Program.Settings.KF2AdminPort}{path}");
		}

		private string AuthorizationHeader => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Program.Settings.KF2AdminUserName}:{Program.Settings.KF2AdminPassword}"));
	}

	public class AdminClientResponse : IResponse
	{
		public HttpStatusCode StatusCode { get; set; }

		public Url Address { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public Stream Content { get; set; }

		public void Dispose()
		{
		}
	}
}
