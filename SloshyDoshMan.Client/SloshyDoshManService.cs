using Serilog;
using SloshyDoshMan.Shared;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace SloshyDoshMan.Client
{
	public interface ISloshyDoshManStatsService
	{
		void SaveGameState(GameState gameState);
	}

	public class SloshyDoshManService : ISloshyDoshManStatsService
	{
		public void SaveGameState(GameState gameState)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient
					.PostAsync(new Uri(SaveGameStatePath), new StringContent(JsonSerializer.Serialize(gameState), Encoding.UTF8, "application/json"))
					.ContinueWith(task =>
					{
						if (task.IsFaulted || task.IsCanceled)
						{
							throw new SloshyDoshManServiceOfflineException();
						}

						return task.Result;
					})
					.Wait();
			}
		}

		public void RegisterServer(RegisterServerRequest request)
		{
			using (var httpClient = new HttpClient())
			{
				Log.Information("Registering Server @ {RegisterServerPath}", RegisterServerPath);

				var response = httpClient
					.PostAsync(new Uri(RegisterServerPath), new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"))
					.ContinueWith(task =>
					{
						if (task.IsFaulted || task.IsCanceled)
						{
							throw new SloshyDoshManServiceOfflineException();
						}

						return task.Result;
					});

				var responseContent = response.Result.Content.ReadAsStringAsync().Result;
				var result = JsonSerializer.Deserialize<Result<Server>>(responseContent);

				if (!result.Success)
				{
					throw new Exception(result.ErrorMessage);
				}

				Log.Information("Registered Server: {ServerId}", result.Data.ServerId);
				Program.ServerId = result.Data.ServerId;
			}
		}

		private static string SaveGameStatePath => LazySaveGameStatePath.Value;
		private static Lazy<string> LazySaveGameStatePath => new Lazy<string>(() => $"http://{Program.Settings.SloshyDoshManServiceHost}:{Program.Settings.SloshyDoshManServicePort}/API/GameState/Save");

		private static string RegisterServerPath => LazyRegisterServerPath.Value;
		private static Lazy<string> LazyRegisterServerPath => new Lazy<string>(() => $"http://{Program.Settings.SloshyDoshManServiceHost}:{Program.Settings.SloshyDoshManServicePort}/API/Servers/Register");
	}
}
