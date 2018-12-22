using SloshyDoshMan.Service.PlayedGames;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.Players
{
	public class PlayerProfile
	{
		public string SteamId { get; set; }
		public string UserName { get; set; }

		public IReadOnlyList<IPlayedGame> AllGames { get; set; }
		public IReadOnlyList<PlayerMapStatistic> MapStatistics { get; set; }
		public IReadOnlyList<PlayerPerkStatistic> PerkStatistics { get; set; }

		public int TotalKills => PerkStatistics.Sum(x => x.TotalKills);
		public int TotalGames => MapStatistics.Sum(x => x.GamesPlayed);
	}
}
