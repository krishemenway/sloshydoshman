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
			return Ok(new ServerStatisticsStore().CalculateStatistics());
		}

		[HttpGet]
		[Route("RecentGames")]
		public IHttpActionResult RecentGames(int count = 10, int startingAt = 0)
		{
			var playedGameStore = new PlayedGameStore();
			var totalGames = playedGameStore.GetTotalGamesCount();
			var recentGames = playedGameStore.FindRecentGames(count, startingAt);

			return Ok(new { TotalGames = totalGames, RecentGames = recentGames});
		}

		[HttpGet]
		[Route("Search")]
		public IHttpActionResult PlayerSearch(string query)
		{
			return Ok(new PlayerStore().Search(query).Select(x => new PlayerViewModel() { UserName = x.Name, SteamId = x.SteamId.ToString(), PerkStatistics = new List<PlayerPerkStatistic>(), MapStatistics = new List<PlayerMapStatistic>() }));
		}
	}
}
