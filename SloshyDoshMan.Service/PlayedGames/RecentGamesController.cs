using Microsoft.AspNetCore.Mvc;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.PlayedGames
{
	[ApiController]
	[Route("WebAPI/Games")]
	public class RecentGamesController : ControllerBase
	{
		public RecentGamesController(IPlayedGameStore playedGameStore = null)
		{
			_playedGameStore = playedGameStore ?? new PlayedGameStore();
		}

		[HttpGet(nameof(Recent))]
		[ProducesResponseType(200, Type = typeof(Result<PlayedGameProfile>))]
		public ActionResult<Result<RecentGamesResponse>> Recent([FromQuery] RecentGamesRequest request)
		{
			var totalGames = _playedGameStore.GetTotalGamesCount();
			var recentGames = _playedGameStore.FindRecentGames(request.Count ?? 10, request.StartingAt ?? 0);

			var response = new RecentGamesResponse
				{
					TotalGames = totalGames,
					RecentGames = recentGames,
				};

			return Result<RecentGamesResponse>.Successful(response);
		}

		private readonly IPlayedGameStore _playedGameStore;
	}
}
