using Dapper;
using SloshyDoshMan.Shared;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.Players
{
	public interface IPlayerStore
	{
		bool TryFindPlayer(long steamId, out Player player);
		IReadOnlyList<Player> Search(string query);

		void SavePlayers(IReadOnlyList<PlayerGameState> players);
	}

	public class PlayerStore : IPlayerStore
	{
		public void SavePlayers(IReadOnlyList<PlayerGameState> players)
		{
			foreach(var player in players)
			{
				SavePlayer(player);
			}
		}

		public IReadOnlyList<Player> Search(string query)
		{
			const string sql = @"
				SELECT
					steam_id as steamid,
					name,
					last_known_ip as LastKnownIP
				FROM player
				WHERE LOWER(name) like LOWER(@Query)
				ORDER BY last_played_time DESC, name ASC";

			using (var connection = Database.CreateConnection())
			{
				return connection.Query<Player>(sql, new { Query = $"%{query}%" }).ToList();
			}
		}

		public bool TryFindPlayer(long steamId, out Player player)
		{
			const string sql = @"
				SELECT
					steam_id as steamid,
					name,
					last_known_ip as LastKnownIP
				FROM player
				WHERE steam_id = @steamid";

			using (var connection = Database.CreateConnection())
			{
				player = connection.QuerySingleOrDefault<Player>(sql, new { steamId });
				return player != null;
			}
		}

		private void SavePlayer(PlayerGameState playerGameState)
		{
			const string sql = @"
				WITH upsert AS (
					UPDATE player 
					SET name=@Name, last_known_ip=@IPAddress, last_played_time=now()
					WHERE steam_id=@SteamID
					RETURNING *
				)

				INSERT INTO player
				(steam_id, name, last_known_ip) 
				SELECT @SteamID, @Name, @IPAddress
				WHERE NOT EXISTS (SELECT * FROM upsert);

				INSERT INTO player_ip 
				(steam_id, ip_address)
				SELECT
					@SteamID as steam_id,
					@IPAddress as ip_address
				WHERE NOT EXISTS (
					SELECT *
					FROM (
						SELECT ip_address
						FROM player_ip
						WHERE steam_id=@SteamID
						ORDER BY started_using_time desc
						LIMIT 1
					) as last_recorded_ip
					WHERE 
						last_recorded_ip.ip_address = @IPAddress
				);";

			using (var connection = Database.CreateConnection())
			{
				connection.Execute(sql, new { playerGameState.SteamId, playerGameState.Name, playerGameState.IPAddress });
			}
		}
	}

	public class Player
	{
		public string Name { get; set; }
		public string LastKnownIP { get; set; }
		public long SteamId { get; set; }
	}
}
