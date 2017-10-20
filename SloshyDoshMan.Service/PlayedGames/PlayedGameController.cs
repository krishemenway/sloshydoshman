using Microsoft.AspNetCore.Mvc;
using System;

namespace SloshyDoshMan.Service.PlayedGames
{
	[Route("webapi")]
	public class PlayedGameController : Controller
	{
		[HttpGet(nameof(Game))]
		public IActionResult Game([FromQuery] Guid playedGameId)
		{
			var playedGame = new PlayedGameStore().FindPlayedGame(playedGameId);

			if(playedGame == null)
			{
				return Ok(Result.Failure("Could not find game"));
			}

			var viewModel = new GameViewModel
			{
				PlayedGame = playedGame,
				Scoreboard = new ScoreboardStore().GetScoreboard(playedGame)
			};

			return Json(Result<GameViewModel>.Successful(viewModel));
		}
	}

	public class GameViewModel
	{
		public IPlayedGame PlayedGame { get; set; }
		public IScoreboard Scoreboard { get; set; }
	}
}
