using Microsoft.AspNetCore.Mvc;
using Serilog;
using SloshyDoshMan.Service.PlayedGames;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.Players
{
	[Route("webapi/players")]
	public class PlayerProfileController : Controller
	{
		public PlayerProfileController(
			IPlayerStatisticsStore playerStatisticsStore = null,
			IPlayedGameStore playedGameStore = null,
			IPlayerStore playerStore = null)
		{
			_playerStatisticsStore = playerStatisticsStore ?? new PlayerStatisticsStore();
			_playedGameStore = playedGameStore ?? new PlayedGameStore();
			_playerStore = playerStore ?? new PlayerStore();
		}

		[HttpGet("profile")]
		[ProducesResponseType(200, Type = typeof(Result<PlayerProfile>))]
		public IActionResult HandleRequest([FromQuery]PlayerProfileRequest request)
		{
			if (!_playerStore.TryFindPlayer(request.SteamId, out var player))
			{
				Log.Information("Unable to find profile for steam id: {SteamId}", request.SteamId);
				return Json(Result<PlayerProfile>.Failure("Unable to find profile for steam id"));
			}

			var playerViewModel = new PlayerProfile
				{
					SteamId = player.SteamId.ToString(),
					UserName = player.Name,
					AllGames = _playedGameStore.FindAllGames(request.SteamId),
					MapStatistics = _playerStatisticsStore.FindMapStatistics(request.SteamId),
					PerkStatistics = _playerStatisticsStore.FindPerkStatistics(request.SteamId),
				};

			return Json(Result<PlayerProfile>.Successful(playerViewModel));
		}

		private readonly IPlayerStatisticsStore _playerStatisticsStore;
		private readonly IPlayedGameStore _playedGameStore;
		private readonly IPlayerStore _playerStore;
	}
}
