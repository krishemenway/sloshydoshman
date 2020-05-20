using Microsoft.AspNetCore.Mvc;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.PlayedGames
{
	[ApiController]
	[Route("WebAPI/Games")]
	public class PlayedGameProfileController : ControllerBase
	{
		public PlayedGameProfileController(
			IPlayedGameStore playedGameStore = null,
			IScoreboardStore scoreboardStore = null)
		{
			_playedGameStore = playedGameStore ?? new PlayedGameStore();
			_scoreboardStore = scoreboardStore ?? new ScoreboardStore();
		}

		[HttpGet(nameof(Profile))]
		[ProducesResponseType(200, Type = typeof(Result<PlayedGameProfile>))]
		public ActionResult<Result<PlayedGameProfile>> Profile([FromQuery] PlayedGameProfileRequest request)
		{
			var playedGame = _playedGameStore.FindPlayedGame(request.PlayedGameId);

			if (playedGame == null)
			{
				return Result<PlayedGameProfile>.Failure($"Could not find game {request.PlayedGameId}");
			}

			var viewModel = new PlayedGameProfile
				{
					PlayedGame = playedGame,
					Scoreboard = _scoreboardStore.GetScoreboard(playedGame)
				};

			return Result<PlayedGameProfile>.Successful(viewModel);
		}

		private readonly IPlayedGameStore _playedGameStore;
		private readonly IScoreboardStore _scoreboardStore;
	}
}
