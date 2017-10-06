using KrisHemenway.Common;
using SloshyDoshMan.PlayedGames;
using SloshyDoshMan.Players;
using SloshyDoshMan.Servers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SloshyDoshMan.WebAPI
{
	[RoutePrefix("webapi/home")]
	public class HomeController : ApiController
	{
		[HttpGet]
		[Route("ServerStats")]
		public IHttpActionResult ServerStatistics()
		{
			var statistics = new ServerStatisticsStore().CalculateStatistics();
			return Ok(Result<ServerStatistics>.Successful(statistics));
		}

		[HttpGet]
		[Route("RecentGames")]
		public IHttpActionResult RecentGames(int count = 10, int startingAt = 0)
		{
			var playedGameStore = new PlayedGameStore();
			var totalGames = playedGameStore.GetTotalGamesCount();
			var recentGames = playedGameStore.FindRecentGames(count, startingAt);
			var response = new RecentGamesResponse { TotalGames = totalGames, RecentGames = recentGames };

			return Ok(Result<RecentGamesResponse>.Successful(response));
		}

		[HttpGet]
		[Route("Search")]
		public IHttpActionResult PlayerSearch(string query)
		{
			var playerSearchResults = new PlayerStore()
				.Search(query)
				.Select(x => new PlayerViewModel() { UserName = x.Name, SteamId = x.SteamId.ToString(), PerkStatistics = new List<PlayerPerkStatistic>(), MapStatistics = new List<PlayerMapStatistic>() })
				.ToList();

			return Ok(Result<IReadOnlyList<PlayerViewModel>>.Successful(playerSearchResults));
		}
	}

	public class RecentGamesResponse
	{
		public int TotalGames { get; set; }
		public IReadOnlyList<IPlayedGame> RecentGames { get; set; }
	}
}
