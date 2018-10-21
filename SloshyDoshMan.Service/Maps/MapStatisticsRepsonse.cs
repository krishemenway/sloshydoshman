using System.Collections.Generic;

namespace SloshyDoshMan.Service.Maps
{
	public class MapStatisticsRepsonse
	{
		public IReadOnlyList<PlayerMapScore> PlayerMapScores { get; set; }
	}
}
