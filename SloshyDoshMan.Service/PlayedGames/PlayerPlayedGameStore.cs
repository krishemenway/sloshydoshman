using Dapper;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.PlayedGames
{
	public interface IPlayerPlayedGameStore
	{
		void RecordPlayersPlayedGame(IPlayedGame playedGame, PlayerGameState playerGameState);
	}

	public class PlayerPlayedGameStore : IPlayerPlayedGameStore
	{
		public void RecordPlayersPlayedGame(IPlayedGame playedGame, PlayerGameState playerGameState)
		{
			const string sql = @"
				WITH upsert AS (
					UPDATE player_played_game 
					SET kills=@Kills
					WHERE 
						steam_id=@SteamID 
						AND played_game_id = @PlayedGameId
					RETURNING *
				)

				INSERT INTO player_played_game
				(steam_id, played_game_id, kills) 
				SELECT @SteamID, @PlayedGameId, @Kills
				WHERE NOT EXISTS (SELECT * FROM upsert);";

			using (var connection = Database.CreateConnection())
			{
				var sqlParams = new
				{
					playedGame.PlayedGameId,
					playerGameState.SteamId,
					playerGameState.Kills
				};

				connection.Execute(sql, sqlParams);
			}
		}
	}
}
