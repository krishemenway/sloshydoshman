using Dapper;
using SloshyDoshMan.Shared;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Maps
{
	public class MapStatisticsStore
	{
		public IReadOnlyList<PlayerMapScore> FindTopPlayersForMap(string mapName)
		{
			const string sql = @"
				SELECT
					p.steam_id as steamid,
					p.name,
					pg.game_difficulty as difficulty,
					COUNT(*) as totalwins
				FROM played_game pg
				INNER JOIN player_played_wave ppw
					ON pg.played_game_id = ppw.played_game_id
					AND pg.reached_wave = ppw.wave
				INNER JOIN player p
					ON ppw.steam_id = p.steam_id
				WHERE
					pg.map = @MapName
					AND pg.players_won = true
				GROUP BY
					p.steam_id,
					p.name,
					pg.game_difficulty";

			using (var connection = Database.CreateConnection())
			{
				return connection.Query<PlayerMapCompleteStatsRow>(sql, new { mapName })
					.GroupBy(x => x.SteamId, x => x)
					.Select(playerDifficultyRows => new PlayerMapScore { SteamId = playerDifficultyRows.Key, Name = playerDifficultyRows.First().Name, TotalScore = playerDifficultyRows.Sum(difficultyRows => difficultyRows.CalculatedScore) })
					.OrderByDescending(x => x.TotalScore)
					.ToList();
			}
		}

		internal static int CalculateScoreForWins(Difficulty difficulty, int totalWins)
		{
			return totalWins * GetDifficultyAdjustmentFactor(difficulty);
		}

		internal static int GetDifficultyAdjustmentFactor(Difficulty difficulty)
		{
			switch(difficulty)
			{
				case Difficulty.Hard:
					return 1;
				case Difficulty.Suicidal:
					return 2;
				case Difficulty.HellOnEarth:
					return 3;
				default:
					return 0;
			}
		}
	}
	
	public class PlayerMapScore
	{
		public long SteamId { get; set; }
		public string Name { get; set; }
		public int TotalScore { get; set; }
	}

	public class PlayerMapCompleteStatsRow
	{
		public long SteamId { get; set; }
		public string Name { get; set; }
		public string Difficulty { get; set; }
		public Difficulty GameDifficulty => DifficultyHelpers.Convert(Difficulty);
		public int TotalWins { get; set; }
		public int CalculatedScore => MapStatisticsStore.CalculateScoreForWins(GameDifficulty, TotalWins);
	}
}
