using Dapper;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.PlayedGames
{
	public interface IPlayerPlayedWaveStore
	{
		void RecordPlayerPlayedWave(IPlayedGame playedGame, PlayerGameState playerGameState);
	}

	public class PlayerPlayedWaveStore : IPlayerPlayedWaveStore
	{
		public void RecordPlayerPlayedWave(IPlayedGame playedGame, PlayerGameState playerGameState)
		{
			const string sql = @"
				WITH upsert AS (
					UPDATE player_played_wave 
					SET kills=@Kills - (SELECT COALESCE(SUM(kills), 0) FROM player_played_wave WHERE steam_id = @SteamId and played_game_id = @PlayedGameId and wave < @Wave)
					WHERE 
						steam_id=@SteamID 
						AND played_game_id = @PlayedGameId
						AND wave = @Wave
					RETURNING *
				)

				INSERT INTO player_played_wave
				(steam_id, played_game_id, wave, kills, perk)
				SELECT @SteamID, @PlayedGameId, @Wave, @Kills - (SELECT COALESCE(SUM(kills), 0) FROM player_played_wave WHERE steam_id = @SteamId and played_game_id = @PlayedGameId and wave < @Wave), @Perk
				WHERE NOT EXISTS (SELECT * FROM upsert);";

			using (var connection = Database.CreateConnection())
			{
				var sqlParams = new
				{
					playedGame.PlayedGameId,
					playerGameState.SteamId,
					playerGameState.Kills,
					playerGameState.Perk,
					Wave = playedGame.ReachedWave
				};

				connection.Execute(sql, sqlParams);
			}
		}
	}
}
