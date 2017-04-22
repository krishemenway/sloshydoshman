using SloshyDoshMan.PlayedGames;
using SloshyDoshMan.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SloshyDoshMan.WebAPI
{
	[RoutePrefix("webapi")]
	public class GameController : ApiController
	{
		[HttpGet]
		[Route("game")]
		public IHttpActionResult Game([FromUri] Guid playedGameId)
		{
			var playedGame = new PlayedGameStore().FindPlayedGame(playedGameId);

			if(playedGame == null)
			{
				return Ok(new { Success = false, ErrorMessage = "Could not find game" });
			}

			var viewModel = new GameViewModel
			{
				PlayedGame = playedGame,
				Scoreboard = new ScoreboardStore().GetScoreboard(playedGame)
			};

			return Ok(viewModel);
		}

		[HttpGet]
		[Route("player")]
		public IHttpActionResult Player([FromUri] long steamId)
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

			return Ok(playerViewModel);
		}
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

	public class GameViewModel
	{
		public IPlayedGame PlayedGame { get; set; }
		public IScoreboard Scoreboard { get; set; }
	}
}
