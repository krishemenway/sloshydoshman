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
				Console.WriteLine(DateTime.Now.ToString("o") + " " + JsonConvert.SerializeObject(gameState));
				var gameStateContent = new StringContent(JsonConvert.SerializeObject(gameState), Encoding.UTF8, "application/json");
				httpClient.PostAsync(new Uri(SaveGameStatePath), gameStateContent).Wait();
			}
		}

		public const string SaveGameStatePath = "http://localhost:8098/api/kf2/gamestate/save";
	}
}
