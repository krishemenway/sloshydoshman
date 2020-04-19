using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.PlayedGames
{
	[ApiController]
	[Route("webapi/games")]
	public class CurrentGamesController : ControllerBase
	{
		public CurrentGamesController(
			IPlayedGameStore playedGameStore = null,
			IScoreboardStore scoreboardStore = null)
		{
			_playedGameStore = playedGameStore ?? new PlayedGameStore();
			_scoreboardStore = scoreboardStore ?? new ScoreboardStore();
		}

		[HttpGet(nameof(CurrentGames))]
		[ProducesResponseType(200, Type = typeof(CurrentGamesResponse))]
		public ActionResult<CurrentGamesResponse> CurrentGames()
		{
			var playedGames = _playedGameStore.AllCurrentGames();
			var scoreboards = _scoreboardStore.GetScoreboards(playedGames);

			return new CurrentGamesResponse
			{
				CurrentGames = playedGames.Select(playedGame => PlayedGameProfile.Create(playedGame, scoreboards)).ToList(),
			};
		}

		private readonly IPlayedGameStore _playedGameStore;
		private readonly IScoreboardStore _scoreboardStore;
	}

	public class CurrentGamesResponse
	{
		public IReadOnlyList<PlayedGameProfile> CurrentGames { get; set; }
	}
}
