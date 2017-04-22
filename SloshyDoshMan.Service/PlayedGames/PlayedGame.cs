using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SloshyDoshMan.Shared;
using System;

namespace SloshyDoshMan.PlayedGames
{
	public interface IPlayedGame
	{
		Guid PlayedGameId { get; }

		string GameType { get; set; }
		string Map { get; set; }
		string Difficulty { get; set; }
		GameLength Length { get; set; }

		int ReachedWave { get; set; }
		int TotalWaves { get; }

		DateTime TimeStarted { get; set; }
		DateTime? TimeFinished { get; set; }
	}

	public class PlayedGame : IPlayedGame
	{
		public Guid PlayedGameId { get; set; }

		public string GameType { get; set; }
		public string Map { get; set; }
		public string Difficulty { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public GameLength Length { get; set; }
		
		public int ReachedWave { get; set; }
		public int TotalWaves => GameLengthHelpers.FindTotalWavesForGameLength(Length);
		public bool PlayersWon { get; set; }

		public DateTime TimeStarted { get; set; }
		public DateTime? TimeFinished { get; set; }
	}
}
