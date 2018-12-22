using Dapper;
using System.Linq;

namespace SloshyDoshMan.Service.PlayedGames
{
	public interface IOverallStatsStore
	{
		OverallStats CalculateStatistics();
	}

	public class OverallStatsStore : IOverallStatsStore
	{
		public OverallStats CalculateStatistics()
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

				return new OverallStats
					{
						Perks = connection.Query<PerkStatistics>(sql).ToList()
					};
			}
		}
	}
}
