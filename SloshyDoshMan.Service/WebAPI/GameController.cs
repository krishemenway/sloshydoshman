using KrisHemenway.Common;
using SloshyDoshMan.PlayedGames;
using System;
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
				return Ok(Result.Failure("Could not find game"));
			}

			var viewModel = new GameViewModel
			{
				PlayedGame = playedGame,
				Scoreboard = new ScoreboardStore().GetScoreboard(playedGame)
			};

			return Ok(Result<GameViewModel>.Successful(viewModel));
		}
	}

	public class GameViewModel
	{
		public IPlayedGame PlayedGame { get; set; }
		public IScoreboard Scoreboard { get; set; }
	}
}
