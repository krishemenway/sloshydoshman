using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using SloshyDoshMan.Service.Extensions;
using SloshyDoshMan.Service.Maps;
using SloshyDoshMan.Service.Notifications;
using SloshyDoshMan.Service.Players;
using SloshyDoshMan.Service.Servers;
using SloshyDoshMan.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.PlayedGames
{
	[Route("API/GameState")]
	public class SaveGameStateController : ControllerBase
	{
		public SaveGameStateController(
			IPlayedGameStore playedGameStore = null, 
			IPlayerStore playerStore = null,
			IPlayerPlayedWaveStore playerPlayedWaveStore = null,
			IPlayerPlayedGameStore playerPlayedGameStore = null,
			IPushNotificationSender pushNotificationSender = null,
			IMapStore mapStore = null,
			IServerStore serverStore = null)
		{
			_playedGameStore = playedGameStore ?? new PlayedGameStore();
			_playerStore = playerStore ?? new PlayerStore();

			_playerPlayedWaveStore = playerPlayedWaveStore ?? new PlayerPlayedWaveStore();
			_playerPlayedGameStore = playerPlayedGameStore ?? new PlayerPlayedGameStore();

			_pushNotificationSender = pushNotificationSender ?? new PushNotificationSender();
			_mapStore = mapStore ?? new MapStore();
			_serverStore = serverStore ?? new ServerStore();
		}

		[HttpPost(nameof(Save))]
		[ProducesResponseType(200, Type = typeof(Result))]
		public ActionResult<Result> Save([FromBody] GameState newGameState)
		{
			if (!_serverStore.TryFindServer(newGameState.ServerId, out _))
			{
				Log.Information("Attempt to save game state with unknown ServerId: {ServerId}", newGameState.ServerId);
				return Result.Failure("Invalid ServerId");
			}

			FixMapName(newGameState);
			RemoveOrFixInvalidPlayers(newGameState);
			SaveAllPlayers(newGameState);

			Log.Debug("Refreshing with Game State: {@GameState}", newGameState);

			if (!_playedGameStore.TryFindCurrentGame(newGameState.ServerId, out var currentGame))
			{
				if (newGameState.CurrentWave == 1)
				{
					if (newGameState.GameType == "Survival")
					{
						currentGame = StartNewGame(newGameState);
						UpdatePlayerRecords(newGameState, currentGame);
					}
					else
					{
						Log.Information("Received State with Unsupported GameType: {GameType} - Ignoring Message.");
						return Result.Successful;
					}
				}
				else
				{
					Log.Debug("No game is running - Ignoring message");
					return Result.Successful;
				}
			}
			else if (MapHasChanged(newGameState, currentGame))
			{
				FinishGame(newGameState, currentGame);
			}
			else
			{
				_playedGameStore.UpdateGame(currentGame, newGameState);
				UpdatePlayerRecords(newGameState, currentGame);

				if (newGameState.CurrentWave > newGameState.TotalWaves)
				{
					var currentPlayerLivingStatus = newGameState.Players.ToDictionary(x => x.SteamId, x => x.Health > 0);
					var newPlayerLivingStatus = FindPlayersWithLivingStatusForServer(newGameState.ServerId).Merge(currentPlayerLivingStatus);

					SteamIdsAliveInFinalWaveByServerIdMemoryCache.Set(newGameState.ServerId, newPlayerLivingStatus);
				}
			}

			return Result.Successful;
		}

		private void SaveAllPlayers(GameState newGameState)
		{
			_playerStore.SavePlayers(newGameState.Players);
		}

		private bool PlayersWonGame(GameState newGameState, IPlayedGame currentGame)
		{
			var reachedBossWave = currentGame.ReachedWave > currentGame.TotalWaves;
			var playersAlive = FindPlayersWithLivingStatusForServer(newGameState.ServerId).Values.Any(isAlive => isAlive);

			return reachedBossWave && playersAlive;
		}

		private bool MapHasChanged(GameState newGameState, IPlayedGame currentGame)
		{
			return newGameState.CurrentWave < currentGame.ReachedWave 
				|| newGameState.Map != currentGame.Map 
				|| newGameState.GameLength != currentGame.Length 
				|| newGameState.Difficulty != currentGame.Difficulty;
		}

		private IPlayedGame StartNewGame(GameState newGameState)
		{
			var newGame = _playedGameStore.StartNewGame(newGameState);
			Log.Information("New Game On Server {ServerId} {Map} {Difficulty} {PlayerCount}", newGameState.ServerId, newGameState.Map, newGameState.Difficulty, newGameState.Players.Count);
			return newGame;
		}

		private void FinishGame(GameState newGameState, IPlayedGame currentGame)
		{
			var playersWon = PlayersWonGame(newGameState, currentGame);
			_playedGameStore.EndGame(currentGame, playersWon);
			Log.Information("Game Finished - {Map} {Difficulty} - {ReachedWave} / {TotalWaves} Won: {PlayersWin}", currentGame.Map, currentGame.Difficulty, currentGame.ReachedWave, currentGame.TotalWaves, playersWon);

			if (playersWon)
			{
				_pushNotificationSender.NotifyAll("SloshyDoshManIncServerUpdate", $"SloshyDoshMan Inc", $"Players won {newGameState.Difficulty} - {newGameState.Map}!");
			}

			SteamIdsAliveInFinalWaveByServerIdMemoryCache.Remove(newGameState.ServerId);
		}

		private void UpdatePlayerRecords(GameState newGameState, IPlayedGame currentGame)
		{
			foreach (var player in newGameState.Players.Where(state => !string.IsNullOrEmpty(state.Perk)))
			{
				_playerPlayedGameStore.RecordPlayersPlayedGame(currentGame, player);
				_playerPlayedWaveStore.RecordPlayerPlayedWave(currentGame, player);
			}
		}

		private void FixMapName(GameState newGameState)
		{
			if (_mapStore.TryFindMap(newGameState.Map, out var map))
			{
				newGameState.Map = map.Name;
			}
		}

		private void RemoveOrFixInvalidPlayers(GameState newGameState)
		{
			newGameState.Players = newGameState.Players.GroupBy(x => x.Name).Select(x => x.First()).ToList();
		}

		private Dictionary<string, bool> FindPlayersWithLivingStatusForServer(Guid serverId) => SteamIdsAliveInFinalWaveByServerIdMemoryCache.GetOrCreate(serverId, (entry) => new Dictionary<string, bool>());

		private readonly IPlayerStore _playerStore;
		private readonly IPlayedGameStore _playedGameStore;

		private readonly IPlayerPlayedGameStore _playerPlayedGameStore;
		private readonly IPlayerPlayedWaveStore _playerPlayedWaveStore;

		private readonly IPushNotificationSender _pushNotificationSender;
		private readonly IMapStore _mapStore;
		private readonly IServerStore _serverStore;

		private static IMemoryCache SteamIdsAliveInFinalWaveByServerIdMemoryCache { get; set; } = new MemoryCache(new MemoryCacheOptions());
	}
}
