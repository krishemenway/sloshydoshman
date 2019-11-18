using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.PlayedGames
{
	public interface IScoreboard
	{
		IReadOnlyList<IScoreboardPlayer> Players { get; }
	}

	public class Scoreboard : IScoreboard
	{
		public int TotalKills => Players.Sum(player => player.PlayerWaveInfo.Sum(wave => wave.Value.Kills));
		public IReadOnlyList<IScoreboardPlayer> Players { get; set; }
	}

	public interface IScoreboardPlayer
	{
		Dictionary<string, PlayerWaveInfo> PlayerWaveInfo { get; set; }
	}

	public class ScoreboardPlayer : IScoreboardPlayer
	{
		public string SteamId { get; set; }
		public string UserName { get; set; }
		public Dictionary<string, PlayerWaveInfo> PlayerWaveInfo { get; set; }
	}

	public interface IScoreboardStore
	{
		IScoreboard GetScoreboard(IPlayedGame game);
	}

	public class ScoreboardStore : IScoreboardStore
	{
		public IScoreboard GetScoreboard(IPlayedGame game)
		{
			const string sql = @"
				SELECT
					p.steam_id as steamid,
					p.name,
					ppw.wave,
					ppw.kills,
					ppw.perk
				FROM player_played_wave ppw
				INNER JOIN player p
					ON p.steam_id = ppw.steam_id
				INNER JOIN played_game pg
					ON ppw.played_game_id = pg.played_game_id
				WHERE
					ppw.played_game_id = @PlayedGameId
					AND wave > 0
					AND perk != ''";

			using (var connection = Database.CreateConnection())
			{
				var playerWaveScoreboards = connection
					.Query<PlayerWaveInfoRecord>(sql, new { game.PlayedGameId })
					.GroupBy(x => x.SteamId, x => x)
					.Select(CreatePlayer)
					.ToList();

				return new Scoreboard
				{
					Players = playerWaveScoreboards
				};
			}
		}

		private ScoreboardPlayer CreatePlayer(IGrouping<long, PlayerWaveInfoRecord> records)
		{
			return new ScoreboardPlayer
			{
				UserName = records.First().Name,
				SteamId = records.First().SteamId.ToString(),
				PlayerWaveInfo = records.ToDictionary(x => x.Wave.ToString(), x => new PlayerWaveInfo { Kills = x.Kills, Perk = x.Perk, Wave = x.Wave })
			};
		}
	}

	public class PlayerWaveInfo
	{
		public int Wave { get; set; }
		public string Perk { get; set; }
		public int Kills { get; set; }
	}

	public class PlayerWaveInfoRecord
	{
		public long SteamId { get; set; }
		public string Name { get; set; }
		public int Wave { get; set; }
		public string Perk { get; set; }
		public int Kills { get; set; }
	}
}
