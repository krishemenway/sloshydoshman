using Microsoft.AspNetCore.Mvc;
using SloshyDoshMan.Service.PlayedGames;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.Players
{
	[Route("webapi")]
	public class PlayerController : Controller
	{
		[HttpGet("player")]
		public IActionResult Player([FromQuery] long steamId)
		{
			var player = new PlayerStore().FindPlayer(steamId);
			var store = new PlayerStatisticsStore();

			var playerViewModel = new PlayerViewModel
			{
				SteamId = player.SteamId.ToString(),
				UserName = player.Name,
				AllGames = new PlayedGameStore().FindAllGames(steamId),
				MapStatistics = store.FindMapStatistics(steamId),
				PerkStatistics = store.FindPerkStatistics(steamId)
			};

			return Json(Result<PlayerViewModel>.Successful(playerViewModel));
		}
	}

	public class MapViewModel
	{
		public string MapName { get; set; }
		public IReadOnlyList<PlayerMapStatistic> PlayerStatistics { get; set; }
	}

	public class PlayerViewModel
	{
		public string SteamId { get; set; }
		public string UserName { get; set; }
		public IReadOnlyList<IPlayedGame> AllGames { get; set; }
		public int TotalKills => PerkStatistics.Sum(x => x.TotalKills);
		public int TotalGames => MapStatistics.Sum(x => x.GamesPlayed);
		public IReadOnlyList<PlayerMapStatistic> MapStatistics { get; set; }
		public IReadOnlyList<PlayerPerkStatistic> PerkStatistics { get; set; }
	}
}
