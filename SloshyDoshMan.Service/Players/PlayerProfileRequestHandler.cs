using SloshyDoshMan.Service.PlayedGames;

namespace SloshyDoshMan.Service.Players
{
	public class PlayerProfileRequestHandler
	{
		public PlayerProfileRequestHandler(
			IPlayerStatisticsStore playerStatisticsStore = null,
			IPlayedGameStore playedGameStore = null,
			IPlayerStore playerStore = null)
		{
			_playerStatisticsStore = playerStatisticsStore ?? new PlayerStatisticsStore();
			_playedGameStore = playedGameStore ?? new PlayedGameStore();
			_playerStore = playerStore ?? new PlayerStore();
		}

		public Result<PlayerProfile> HandleRequest(PlayerProfileRequest request)
		{
			var player = _playerStore.FindPlayer(request.SteamId);

			var playerViewModel = new PlayerProfile
				{
					SteamId = player.SteamId.ToString(),
					UserName = player.Name,
					AllGames = _playedGameStore.FindAllGames(request.SteamId),
					MapStatistics = _playerStatisticsStore.FindMapStatistics(request.SteamId),
					PerkStatistics = _playerStatisticsStore.FindPerkStatistics(request.SteamId)
				};

			return Result<PlayerProfile>.Successful(playerViewModel);
		}

		private readonly IPlayerStatisticsStore _playerStatisticsStore;
		private readonly IPlayedGameStore _playedGameStore;
		private readonly IPlayerStore _playerStore;
	}
}
