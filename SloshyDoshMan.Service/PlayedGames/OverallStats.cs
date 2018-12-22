using System.Collections.Generic;

namespace SloshyDoshMan.Service.PlayedGames
{
	public class OverallStats
	{
		public IReadOnlyList<PerkStatistics> Perks { get; set; }
	}

	public class PerkStatistics
	{
		public string PerkName { get; set; }
		public int TotalWavesPlayed { get; set; }
		public int TotalKills { get; set; }
	}
}
