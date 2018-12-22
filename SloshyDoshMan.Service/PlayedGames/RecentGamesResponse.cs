using System.Collections.Generic;

namespace SloshyDoshMan.Service.PlayedGames
{
	public class RecentGamesResponse
	{
		public int TotalGames { get; set; }
		public IReadOnlyList<IPlayedGame> RecentGames { get; set; }
	}
}
