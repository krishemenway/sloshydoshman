using System;
using System.Collections.Generic;

namespace SloshyDoshMan.Service.PlayedGames
{
	public class PlayedGameProfile
	{
		public IPlayedGame PlayedGame { get; set; }
		public IScoreboard Scoreboard { get; set; }

		public static PlayedGameProfile Create(IPlayedGame playedGame, IReadOnlyDictionary<Guid, IScoreboard> scoreboardsByPlayedGameId) 
		{
			return new PlayedGameProfile
			{
				PlayedGame = playedGame,
				Scoreboard = scoreboardsByPlayedGameId[playedGame.PlayedGameId],
			};
		}
	}
}
