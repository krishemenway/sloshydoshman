using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Servers
{
	public class ServerStatisticsStore
	{
		public ServerStatistics CalculateStatistics()
		{
			using(var connection = Database.CreateConnection())
			{
				const string sql = @"
					SELECT
						ppw.perk as perkname,
						COUNT(ppw.steam_id) as totalwavesplayed,
						SUM(ppw.kills) as totalkills
					FROM player_played_wave ppw
					WHERE 
						ppw.perk !='\'
						AND ppw.perk IS NOT NULL
					GROUP BY ppw.perk
					ORDER BY perk";

				return new ServerStatistics
				{
					Perks = connection.Query<PerkStatistics>(sql).ToList()
				};
			}
		}
	}

	public class ServerStatistics
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
