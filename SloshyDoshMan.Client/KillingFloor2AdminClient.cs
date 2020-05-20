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
		void ChangeMap(string map);
	}

	public class KillingFloor2AdminClient : IKillingFloor2AdminClient
	{
		public KillingFloor2AdminClient(ISettings settings = null)
		{
			_settings = settings ?? Program.Settings;
		}

		public void SendMessage(string message)
		{
			PostContentToUri(CreateKF2AdminServerPath("/ServerAdmin/current/chat+frame+data"), $"message={HttpUtility.UrlEncode(message)}&teamsay=-1");
		}

		public void ChangeMap(string map)
		{
			var uri = CreateKF2AdminServerPath("/ServerAdmin/current/change");
			var postContent = $"gametype=KFGameContent.KFGameInfo_Survival&map={map}&mutatorGroupCount=0&urlextra={HttpUtility.UrlEncode(FindUrlExtraForChangeMap())}&action=change";

			PostContentToUri(uri, postContent);
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

		private void PostContentToUri(Uri uri, string formContent)
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthorizationHeader);

				var requestTask = client
					.PostAsync(uri, new StringContent(formContent, Encoding.UTF8, "application/x-www-form-urlencoded"))
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

		private string FindUrlExtraForChangeMap()
		{
			var content = BrowsingContext.New().OpenAsync(GetContentFromUri(CreateKF2AdminServerPath("/ServerAdmin/current/change"))).Result;
			return content.QuerySelector("#urlextra").GetAttribute("value");
		}

		private Uri CreateKF2AdminServerPath(string path)
		{
			return new Uri($"http://{_settings.KF2AdminHost}:{_settings.KF2AdminPort}{path}");
		}

		private string AuthorizationHeader => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.KF2AdminUserName}:{_settings.KF2AdminPassword}"));

		private ISettings _settings;
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
