using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.PlayedGames
{
	[ApiController]
	[Route("WebAPI/Games")]
	public class RecentWinsController : ControllerBase
	{
		public RecentWinsController(
			IPlayedGameStore playedGameStore = null,
			IScoreboardStore scoreboardStore = null)
		{
			_playedGameStore = playedGameStore ?? new PlayedGameStore();
			_scoreboardStore = scoreboardStore ?? new ScoreboardStore();
		}

		[HttpGet(nameof(RecentWins))]
		[ProducesResponseType(200, Type = typeof(RecentWinsResponse))]
		public ActionResult<RecentWinsResponse> RecentWins()
		{
			var recentWins = _playedGameStore.FindRecentWins(5);
			var scoreboards = _scoreboardStore.GetScoreboards(recentWins);

			return new RecentWinsResponse
			{
				RecentWins = recentWins.Select(playedGame => PlayedGameProfile.Create(playedGame, scoreboards)).ToList(),
			};
		}

		private readonly IPlayedGameStore _playedGameStore;
		private readonly IScoreboardStore _scoreboardStore;
	}

	public class RecentWinsResponse
	{
		public IReadOnlyList<PlayedGameProfile> RecentWins { get; set; }
	}
}
