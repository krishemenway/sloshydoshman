using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.Players
{
	public interface IPlayerStatisticsStore
	{
		IReadOnlyList<PlayerMapStatistic> FindMapStatistics(long steamId);
		IReadOnlyList<PlayerPerkStatistic> FindPerkStatistics(long steamId);
	}

	public class PlayerStatisticsStore : IPlayerStatisticsStore
	{
		public IReadOnlyList<PlayerMapStatistic> FindMapStatistics(long steamId)
		{
			const string sql = @"
				SELECT
					allmaps.map,
					allmaps.isworkshop,
					allmaps.difficulty,
					userstats.steamid,
					userstats.gamesplayed,
					userstats.totalkills,
					userstats.farthestwave,
					gamestats.gameswon
				FROM (
					SELECT
						m.map,
						m.is_workshop as isworkshop,
						d.difficulty
					FROM server_map m
					CROSS JOIN server_difficulties d
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
				) AS gamestats ON lower(allmaps.difficulty) = lower(gamestats.game_difficulty) AND lower(allmaps.map) = lower(gamestats.map)
				LEFT JOIN (
					SELECT
						ppg.steam_id as steamid,
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
					GROUP BY ppg.steam_id, pg.map, pg.game_difficulty
				) AS userstats ON allmaps.difficulty = userstats.difficulty AND lower(allmaps.map) = lower(userstats.map)";

			using (var connection = Database.CreateConnection())
			{
				return connection
					.Query<PlayerMapStatistic>(sql, new { steamId })
					.Where(x => !x.IsWorkshop || x.GamesPlayed > 0)
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
