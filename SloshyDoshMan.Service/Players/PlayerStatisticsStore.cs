using Dapper;
using SloshyDoshMan.Service.Extensions;
using SloshyDoshMan.Service.Maps;
using SloshyDoshMan.Service.Perks;
using SloshyDoshMan.Shared;
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
		public PlayerStatisticsStore(
			IMapStore mapStore = null,
			IPerkStore perkStore = null)
		{
			_mapStore = mapStore ?? new MapStore();
			_perkStore = perkStore ?? new PerkStore();
		}

		public IReadOnlyList<PlayerMapStatistic> FindMapStatistics(long steamId)
		{
			const string sql = @"
				SELECT
					userstats.map,
					userstats.difficulty,
					userstats.gamesplayed,
					userstats.totalkills,
					userstats.farthestwave,
					gamestats.gameswon
				FROM (
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
				) AS userstats
				LEFT JOIN (
					SELECT
						pg.map,
						pg.game_difficulty as difficulty,
						COUNT(*) as gameswon
					FROM player_played_wave ppw
					INNER JOIN played_game pg
						ON ppw.played_game_id = pg.played_game_id
						AND ppw.wave > pg.total_waves
						AND pg.players_won = true
					WHERE 
						steam_id = @SteamId
					GROUP BY pg.map, pg.game_difficulty
				) AS gamestats ON gamestats.difficulty = userstats.difficulty AND lower(gamestats.map) = lower(userstats.map)
				WHERE 
					userstats.difficulty != 'Normal'";

			using (var connection = Database.CreateConnection())
			{
				return connection
					.Query<PlayerMapStatistic>(sql, new { steamId })
					.ToDictionary(mapDifficultyStats => (MapName: mapDifficultyStats.Map, Difficulty: DifficultyHelpers.Convert(mapDifficultyStats.Difficulty)), mapDifficultyStats => mapDifficultyStats)
					.SetDefaultValuesForKeys(_mapStore.FindCoreMapDifficulties(), (mapDifficulty) => CreatePlayerMapStatistics(mapDifficulty.MapName, mapDifficulty.Difficulty))
					.Select(keyValuePair => keyValuePair.Value)
					.OrderBy(x => x.Map).ThenBy(x => x.GameDifficulty)
					.ToList();
			}
		}

		public IReadOnlyList<PlayerPerkStatistic> FindPerkStatistics(long steamId)
		{
			const string sql = @"
				SELECT 
					ppw.perk,
					COUNT(CASE WHEN ppw.wave IS NOT NULL THEN 1 ELSE NULL END) as totalwavesplayed,
					SUM(CASE WHEN ppw.kills IS NULL THEN 0 ELSE ppw.kills END) as totalkills
				FROM player_played_wave ppw
				WHERE ppw.steam_id = @SteamId
				GROUP BY ppw.perk
				ORDER BY totalwavesplayed DESC";
			
			using (var connection = Database.CreateConnection())
			{
				var allPerks = _perkStore.FindAllPerks();

				return connection
					.Query<PlayerPerkStatistic>(sql, new { steamId })
					.GroupBy(playerPerkStatistic => playerPerkStatistic.Perk, playerPerkStatistic => playerPerkStatistic)
					.ToDictionary(playerPerkStatistic => playerPerkStatistic.Key, playerPerkStatistics => playerPerkStatistics.Single())
					.SetDefaultValuesForKeys(allPerks.Select(x => x.Name), name => new PlayerPerkStatistic { Perk = name })
					.Select(x => x.Value)
					.ToList();
			}
		}

		private PlayerMapStatistic CreatePlayerMapStatistics(string mapName, Difficulty difficulty)
		{
			return new PlayerMapStatistic
				{
					Map = mapName,
					Difficulty = DifficultyHelpers.Convert(difficulty),
					FarthestWave = 0,
					GamesPlayed = 0,
					GamesWon = 0,
					TotalKills = 0,
				};
		}

		private readonly IMapStore _mapStore;
		private readonly IPerkStore _perkStore;
	}
}
