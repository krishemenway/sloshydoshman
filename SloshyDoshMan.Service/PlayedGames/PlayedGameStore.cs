using Dapper;
using System;
using System.Linq;
using SloshyDoshMan.Shared;
using System.Collections.Generic;

namespace SloshyDoshMan.Service.PlayedGames
{
	public interface IPlayedGameStore
	{
		IPlayedGame FindPlayedGame(Guid playedGameId);
		bool TryFindCurrentGame(Guid serverId, out IPlayedGame playedGame);
		IReadOnlyList<IPlayedGame> FindRecentGames(int totalRecentGames, int startingAt);
		IReadOnlyList<IPlayedGame> FindRecentWins(int countToFind);
		IReadOnlyList<IPlayedGame> FindAllGames(long steamId);

		IPlayedGame StartNewGame(GameState newGameState);
		void EndGame(IPlayedGame currentPlayedGame, bool playersWon);
		void UpdateGame(IPlayedGame playedGame, GameState newGameState);

		int GetTotalGamesCount();
	}

	public class PlayedGameStore : IPlayedGameStore
	{
		public int GetTotalGamesCount()
		{
			const string sql = @"
				SELECT COUNT(*) 
				FROM played_game pg
				WHERE EXISTS (
					SELECT * 
					FROM player_played_wave ppw
					WHERE pg.played_game_id = ppw.played_game_id
					AND wave > 0
				)";

			using (var connection = Database.CreateConnection())
			{
				return connection.QuerySingle<int>(sql);
			}
		}

		public bool TryFindCurrentGame(Guid serverId, out IPlayedGame playedGame)
		{
			const string sql = @"
				SELECT
					played_game_id as PlayedGameId,
					map,
					game_type as GameType,
					game_length as GameLength,
					game_difficulty as GameDifficulty,
					reached_wave as ReachedWave,
					time_started as TimeStarted,
					time_finished as TimeFinished,
					players_won as PlayersWon
				FROM played_game
				WHERE 
					server_id = @ServerId
					AND time_finished IS NULL
				ORDER BY
					time_started DESC
				LIMIT 1";

			using (var connection = Database.CreateConnection())
			{
				playedGame = connection
					.Query<PlayedGameRecord>(sql, new { serverId })
					.Select(CreateGame)
					.SingleOrDefault();

				return playedGame != null;
			}
		}

		public IPlayedGame FindPlayedGame(Guid playedGameId)
		{
			const string sql = @"
				SELECT
					played_game_id as PlayedGameId,
					map,
					game_type as GameType,
					game_length as GameLength,
					game_difficulty as GameDifficulty,
					reached_wave as ReachedWave,
					time_started as TimeStarted,
					time_finished as TimeFinished,
					players_won as PlayersWon
				FROM played_game
				WHERE played_game_id = @PlayedGameId
				ORDER BY time_started DESC
				LIMIT 1";

			using (var connection = Database.CreateConnection())
			{
				return connection
					.Query<PlayedGameRecord>(sql, new { playedGameId })
					.Select(CreateGame)
					.SingleOrDefault();
			}
		}

		public IReadOnlyList<IPlayedGame> FindAllGames(long steamId)
		{
			const string sql = @"
				SELECT
					pg.played_game_id as PlayedGameId,
					pg.map,
					pg.game_type as GameType,
					pg.game_length as GameLength,
					pg.game_difficulty as GameDifficulty,
					pg.reached_wave as ReachedWave,
					pg.time_started as TimeStarted,
					pg.time_finished as TimeFinished,
					pg.players_won as PlayersWon
				FROM played_game pg
				INNER JOIN player_played_game ppg
					ON pg.played_game_id = ppg.played_game_id
				WHERE ppg.steam_id = @SteamId
					AND ppg.kills > 0
				ORDER BY pg.time_started DESC";

			using (var connection = Database.CreateConnection())
			{
				return connection
					.Query<PlayedGameRecord>(sql, new { steamId })
					.Select(CreateGame)
					.ToList();
			}
		}

		public IReadOnlyList<IPlayedGame> FindRecentWins(int countToFind)
		{
			const string sql = @"
				SELECT
					played_game_id as PlayedGameId,
					map,
					game_type as GameType,
					game_length as GameLength,
					game_difficulty as GameDifficulty,
					reached_wave as ReachedWave,
					time_started as TimeStarted,
					time_finished as TimeFinished,
					players_won as PlayersWon
				FROM played_game pg
				WHERE pg.players_won = true
				ORDER BY pg.time_started DESC
				LIMIT @CountToFind";

			using (var connection = Database.CreateConnection())
			{
				return connection
					.Query<PlayedGameRecord>(sql, new { CountToFind = countToFind })
					.Select(CreateGame)
					.ToList();
			}
		}

		public IReadOnlyList<IPlayedGame> FindRecentGames(int totalRecentGames, int startingAt)
		{
			const string sql = @"
				SELECT
					played_game_id as PlayedGameId,
					map,
					game_type as GameType,
					game_length as GameLength,
					game_difficulty as GameDifficulty,
					reached_wave as ReachedWave,
					time_started as TimeStarted,
					time_finished as TimeFinished,
					players_won as PlayersWon
				FROM played_game pg
				WHERE EXISTS (
					SELECT * 
					FROM player_played_wave ppw
					WHERE pg.played_game_id = ppw.played_game_id
					AND wave > 0
				)
				ORDER BY pg.time_started DESC
				LIMIT @RecentGameCount
				OFFSET @StartingAt";

			using (var connection = Database.CreateConnection())
			{
				return connection
					.Query<PlayedGameRecord>(sql, new { RecentGameCount = totalRecentGames, StartingAt = startingAt })
					.Select(CreateGame)
					.ToList();
			}
		}

		public void EndGame(IPlayedGame currentPlayedGame, bool playersWon)
		{
			const string sql = @"
				UPDATE played_game
				SET 
					time_finished = now(),
					players_won = @PlayersWon
				WHERE
					played_game_id = @PlayedGameId
					AND time_finished IS NULL";

			using (var connection = Database.CreateConnection())
			{
				connection.Execute(sql, new { currentPlayedGame.PlayedGameId, playersWon });
			}
		}

		public IPlayedGame StartNewGame(GameState newGameState)
		{
			const string sql = @"
				INSERT INTO played_game
				(played_game_id, map, game_type, game_length, game_difficulty, reached_wave, total_waves, server_id)
				VALUES (uuid_generate_v4(), @Map, @GameType, @GameLength, @Difficulty, @ReachedWave, @TotalWaves, @ServerId)
				RETURNING played_game_id; ";

			using (var connection = Database.CreateConnection())
			{
				var sqlParams = new
				{
					newGameState.Map,
					newGameState.GameType,
					GameLength = newGameState.GameLength.ToString("G"),
					newGameState.Difficulty,
					ReachedWave = newGameState.CurrentWave,
					newGameState.TotalWaves,
					newGameState.ServerId
				};

				return FindPlayedGame(connection.QuerySingle<Guid>(sql, sqlParams));
			}
		}

		public void UpdateGame(IPlayedGame playedGame, GameState newGameState)
		{
			const string sql = @"
				UPDATE played_game
				SET reached_wave = @ReachedWave
				WHERE played_game_id = @PlayedGameId
				AND reached_wave != @ReachedWave";

			using (var connection = Database.CreateConnection())
			{
				var sqlParams = new
				{
					playedGame.PlayedGameId,
					ReachedWave = newGameState.CurrentWave
				};

				connection.Execute(sql, sqlParams);
			}
		}

		private static IPlayedGame CreateGame(PlayedGameRecord gameRecord)
		{
			return new PlayedGame
			{
				PlayedGameId = gameRecord.PlayedGameId,

				Map = gameRecord.Map,
				Difficulty = gameRecord.GameDifficulty,
				GameType = gameRecord.GameType,

				ReachedWave = gameRecord.ReachedWave,
				Length = (GameLength)Enum.Parse(typeof(GameLength), gameRecord.GameLength),

				TimeStarted = gameRecord.TimeStarted,
				TimeFinished = gameRecord.TimeFinished,

				PlayersWon = gameRecord.PlayersWon
			};
		}
	}

	public class PlayedGameRecord
	{
		public Guid PlayedGameId { get; set; }
		public string Map { get; set; }
		public string GameType { get; set; }
		public string GameDifficulty { get; set; }
		public string GameLength { get; set; }
		public int ReachedWave { get; set; }
		public DateTime TimeStarted { get; set; }
		public DateTime? TimeFinished { get; set; }
		public bool PlayersWon { get; set; }
	}
}
