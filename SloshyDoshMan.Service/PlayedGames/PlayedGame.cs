using SloshyDoshMan.Shared;
using System;

namespace SloshyDoshMan.Service.PlayedGames
{
	public interface IPlayedGame
	{
		Guid PlayedGameId { get; }

		string GameType { get; }
		string Map { get; }
		string Difficulty { get; }

		GameLength Length { get; }

		int ReachedWave { get; }
		int TotalWaves { get; }
		bool PlayersWon { get; }

		DateTime TimeStarted { get; set; }
		DateTime? TimeFinished { get; set; }
	}

	public class PlayedGame : IPlayedGame
	{
		public Guid PlayedGameId { get; set; }

		public string GameType { get; set; }
		public string Map { get; set; }
		public string Difficulty { get; set; }

		public GameLength Length { get; set; }
		
		public int ReachedWave { get; set; }
		public int TotalWaves => GameLengthHelpers.FindTotalWavesForGameLength(Length);
		public bool PlayersWon { get; set; }

		public DateTime TimeStarted { get; set; }
		public DateTime? TimeFinished { get; set; }
	}
}
