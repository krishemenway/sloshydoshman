using Microsoft.AspNetCore.Mvc;
using SloshyDoshMan.Service.Servers;
using SloshyDoshMan.Service.PlayedGames;
using SloshyDoshMan.Service.Players;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service
{
	[Route("webapi/home")]
	public class HomeController : Controller
	{
		[HttpGet("ServerStats")]
		public IActionResult ServerStatistics()
		{
			var statistics = new ServerStatisticsStore().CalculateStatistics();
			return Json(Result<ServerStatistics>.Successful(statistics));
		}

		[HttpGet("RecentGames")]
		public IActionResult RecentGames(int count = 10, int startingAt = 0)
		{
			var playedGameStore = new PlayedGameStore();
			var totalGames = playedGameStore.GetTotalGamesCount();
			var recentGames = playedGameStore.FindRecentGames(count, startingAt);
			var response = new RecentGamesResponse { TotalGames = totalGames, RecentGames = recentGames };

			return Json(Result<RecentGamesResponse>.Successful(response));
		}

		[HttpGet("Search")]
		public IActionResult PlayerSearch(string query)
		{
			var playerSearchResults = new PlayerStore()
				.Search(query)
				.Select(x => new PlayerViewModel { UserName = x.Name, SteamId = x.SteamId.ToString(), PerkStatistics = new List<PlayerPerkStatistic>(), MapStatistics = new List<PlayerMapStatistic>() })
				.ToList();

			return Json(Result<IReadOnlyList<PlayerViewModel>>.Successful(playerSearchResults));
		}
	}

	public class RecentGamesResponse
	{
		public int TotalGames { get; set; }
		public IReadOnlyList<IPlayedGame> RecentGames { get; set; }
	}
}
