using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.Players
{
	public class PlayerMapStatistic
	{
		public string Map { get; set; }
		public string Difficulty { get; set; }
		public Difficulty GameDifficulty => DifficultyHelpers.Convert(Difficulty);
		public int GamesPlayed { get; set; }
		public int GamesWon { get; set; }
		public int TotalKills { get; set; }
		public int FarthestWave { get; set; }
	}
}
