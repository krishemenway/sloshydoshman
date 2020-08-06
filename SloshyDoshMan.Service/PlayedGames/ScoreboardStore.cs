using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.PlayedGames
{
	public interface IScoreboard
	{
		int TotalKills { get; }
		IReadOnlyList<IScoreboardPlayer> Players { get; }
	}

	public class Scoreboard : IScoreboard
	{
		public int TotalKills => Players.Sum(player => player.PlayerWaveInfo.Sum(wave => wave.Value.Kills));
		public IReadOnlyList<IScoreboardPlayer> Players { get; set; }
	}

	public interface IScoreboardPlayer
	{
		string SteamId { get; set; }
		string UserName { get; set; }
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
		Dictionary<Guid, IScoreboard> GetScoreboards(IReadOnlyList<IPlayedGame> playedGames);
	}

	public class ScoreboardStore : IScoreboardStore
	{
		public IScoreboard GetScoreboard(IPlayedGame game)
		{
			const string sql = @"
				SELECT
					ppw.played_game_id playedgameid,
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
		
		public Dictionary<Guid, IScoreboard> GetScoreboards(IReadOnlyList<IPlayedGame> playedGames)
		{
			const string sql = @"
				SELECT
					ppw.played_game_id playedgameid,
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
					ppw.played_game_id = ANY(@PlayedGameIds)
					AND wave > 0
					AND perk != ''";

			using (var connection = Database.CreateConnection())
			{
				return connection
					.Query<PlayerWaveInfoRecord>(sql, new { PlayedGameIds = playedGames.Select(x => x.PlayedGameId).ToList() })
					.GroupBy(x => x.PlayedGameId, x => x)
					.ToDictionary(groupByGame => groupByGame.Key, playerWaveInfoRecordsForGame => CreateScoreboard(playerWaveInfoRecordsForGame.ToList()));
			}
		}

		private IScoreboard CreateScoreboard(List<PlayerWaveInfoRecord> records)
		{
			var playerWaveScoreboards = records
				.GroupBy(x => x.SteamId, x => x)
				.Select(CreatePlayer)
				.ToList();

			return new Scoreboard
			{
				Players = playerWaveScoreboards
			};
		}

		private ScoreboardPlayer CreatePlayer(IGrouping<string, PlayerWaveInfoRecord> records)
		{
			return new ScoreboardPlayer
			{
				UserName = records.First().Name,
				SteamId = records.First().SteamId,
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
		public Guid PlayedGameId { get; set; }
		public string SteamId { get; set; }
		public string Name { get; set; }
		public int Wave { get; set; }
		public string Perk { get; set; }
		public int Kills { get; set; }
	}
}
