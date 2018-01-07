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

	public class SloshyDoshManStatsService : ISloshyDoshManStatsService
	{
		public void SaveGameState(GameState gameState)
		{
			using (var httpClient = new HttpClient())
			{
				var serializedGameState = JsonConvert.SerializeObject(gameState);

				Program.LoggerFactory.CreateLogger<SloshyDoshManStatsService>().LogInformation($"Received Game State: {serializedGameState}");
				httpClient.PostAsync(new Uri(SaveGameStatePath), new StringContent(serializedGameState, Encoding.UTF8, "application/json")).Wait();
			}
		}

		private static string SaveGameStatePath => LazySaveGameStatePath.Value;
		private static Lazy<string> LazySaveGameStatePath => new Lazy<string>(() => $"http://{Program.Settings.SloshyDoshManServiceHost}:{Program.Settings.SloshyDoshManServicePort}/api/kf2/gamestate/save");
	}
}
