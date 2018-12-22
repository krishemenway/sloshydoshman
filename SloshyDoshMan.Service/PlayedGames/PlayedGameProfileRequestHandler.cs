namespace SloshyDoshMan.Service.PlayedGames
{
	public class PlayedGameProfileRequestHandler
	{
		public Result<PlayedGameProfile> HandleRequest(PlayedGameProfileRequest request)
		{
			var playedGame = new PlayedGameStore().FindPlayedGame(request.PlayedGameId);

			if (playedGame == null)
			{
				return Result<PlayedGameProfile>.Failure("Could not find game");
			}

			var viewModel = new PlayedGameProfile
				{
					PlayedGame = playedGame,
					Scoreboard = new ScoreboardStore().GetScoreboard(playedGame)
				};

			return Result<PlayedGameProfile>.Successful(viewModel);
		}
	}
}
