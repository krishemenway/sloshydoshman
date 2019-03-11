using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SloshyDoshMan.Shared;
using System;
using System.Net.Http;
using System.Text;

namespace SloshyDoshMan.Client
{
	public interface ISloshyDoshManStatsService
	{
		void SaveGameState(GameState gameState);
	}

	public class SloshyDoshManService : ISloshyDoshManStatsService
	{
		public SloshyDoshManService(ILogger<SloshyDoshManService> logger = null)
		{
			_logger = logger ?? Program.LoggerFactory.CreateLogger<SloshyDoshManService>();
		}

		public void SaveGameState(GameState gameState)
		{
			using (var httpClient = new HttpClient())
			{
				var serializedGameState = JsonConvert.SerializeObject(gameState);

				_logger.LogInformation($"Received Game State: {serializedGameState}");

				httpClient
					.PostAsync(new Uri(SaveGameStatePath), new StringContent(serializedGameState, Encoding.UTF8, "application/json"))
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
				_logger.LogInformation($"Registering Server");

				var response = httpClient
					.PostAsync(new Uri(RegisterServerPath), new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"))
					.ContinueWith(task =>
					{
						if (task.IsFaulted || task.IsCanceled)
						{
							throw new SloshyDoshManServiceOfflineException();
						}

						return task.Result;
					});

				var responseContent = response.Result.Content.ReadAsStringAsync().Result;
				var result = JsonConvert.DeserializeObject<Result<Server>>(responseContent);

				if (!result.Success)
				{
					throw new Exception(result.ErrorMessage);
				}

				_logger.LogInformation($"Registered Server: {result.Data.ServerId}");
				Program.ServerId = result.Data.ServerId;
			}
		}

		private static string SaveGameStatePath => LazySaveGameStatePath.Value;
		private static Lazy<string> LazySaveGameStatePath => new Lazy<string>(() => $"http://{Program.Settings.SloshyDoshManServiceHost}:{Program.Settings.SloshyDoshManServicePort}/api/kf2/gamestate/save");

		private static string RegisterServerPath => LazyRegisterServerPath.Value;
		private static Lazy<string> LazyRegisterServerPath => new Lazy<string>(() => $"http://{Program.Settings.SloshyDoshManServiceHost}:{Program.Settings.SloshyDoshManServicePort}/api/servers/register");

		private readonly ILogger<SloshyDoshManService> _logger;
	}
}
