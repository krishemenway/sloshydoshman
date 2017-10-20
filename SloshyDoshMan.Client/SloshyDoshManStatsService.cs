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

		public const string SaveGameStatePath = "http://localhost:8098/api/kf2/gamestate/save";
	}
}
