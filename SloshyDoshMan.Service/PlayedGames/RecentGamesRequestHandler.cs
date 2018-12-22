namespace SloshyDoshMan.Service.PlayedGames
{
	public class RecentGamesRequestHandler
	{
		public RecentGamesRequestHandler(IPlayedGameStore playedGameStore = null)
		{
			_playedGameStore = playedGameStore ?? new PlayedGameStore();
		}

		public Result<RecentGamesResponse> HandleRequest(RecentGamesRequest request)
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
