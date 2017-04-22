using Dapper;
using SloshyDoshMan.Shared;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Players
{
	public class PlayerPerkStatistic
	{
		public string Perk { get; set; }
		public int TotalWavesPlayed { get; set; }
		public int TotalKills { get; set; }
	}

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

	public class PlayerStatisticsStore
	{
		public IReadOnlyList<PlayerMapStatistic> FindMapStatistics(long steamId)
		{
			const string sql = @"
				SELECT
					allmaps.map,
					allmaps.difficulty,
					userstats.gamesplayed,
					userstats.totalkills,
					userstats.farthestwave,
					gamestats.gameswon
				FROM (
					SELECT
						m.map,
						d.difficulty
					FROM server_map m
					CROSS JOIN server_difficulties d
					WHERE is_workshop = false  
				) as allmaps
				LEFT JOIN (
					SELECT
						pg.map,
						pg.game_difficulty,
						COUNT(*) as gameswon
					FROM player_played_wave ppw
					INNER JOIN played_game pg
						ON ppw.played_game_id = pg.played_game_id
						AND ppw.wave > pg.total_waves
						AND pg.players_won = true
					WHERE 
						steam_id = @SteamId
					GROUP BY pg.map, pg.game_difficulty
				) AS gamestats ON allmaps.difficulty = gamestats.game_difficulty AND allmaps.map = gamestats.map
				LEFT JOIN (
					SELECT
						pg.map,
						pg.game_difficulty as difficulty,
						COUNT(DISTINCT ppw.played_game_id) as gamesplayed,
    					SUM(CASE WHEN pg.players_won THEN 1 ELSE 0 END) as gameswon,
						SUM(COALESCE(ppw.kills,0)) as totalkills,
						MAX(pg.reached_wave) as farthestwave
					FROM player_played_wave ppw
					INNER JOIN player_played_game ppg
						ON ppw.played_game_id = ppg.played_game_id
						AND ppw.steam_id = ppg.steam_id
					INNER JOIN played_game pg
						on pg.played_game_id = ppw.played_game_id
					WHERE
						ppg.steam_id = @SteamId
					GROUP BY pg.map, pg.game_difficulty
				) AS userstats ON allmaps.difficulty = userstats.difficulty AND allmaps.map = userstats.map";

			using (var connection = Database.CreateConnection())
			{
				return connection
					.Query<PlayerMapStatistic>(sql, new { steamId })
					.OrderBy(x => x.Map).ThenBy(x => x.GameDifficulty)
					.ToList()
					.AsReadOnly();
			}
		}

		public IReadOnlyList<PlayerPerkStatistic> FindPerkStatistics(long steamId)
		{
			const string sql = @"
				SELECT 
					sp.name as perk,
					COUNT(CASE WHEN ppw.wave IS NOT NULL THEN 1 ELSE NULL END) as totalwavesplayed,
					SUM(CASE WHEN kills IS NULL THEN 0 ELSE kills END) as totalkills
				FROM server_perk sp
				LEFT OUTER JOIN player_played_wave ppw
					ON sp.name = ppw.perk
					AND (ppw.steam_id = @SteamId OR ppw.steam_id IS NULL)
				GROUP BY sp.name";
			
			using (var connection = Database.CreateConnection())
			{
				return connection.Query<PlayerPerkStatistic>(sql, new { steamId }).ToList().AsReadOnly();
			}
		}
	}
}
