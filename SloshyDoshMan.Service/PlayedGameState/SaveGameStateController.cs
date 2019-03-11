using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SloshyDoshMan.Service.Maps;
using SloshyDoshMan.Service.Notifications;
using SloshyDoshMan.Service.PlayedGames;
using SloshyDoshMan.Service.Players;
using SloshyDoshMan.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.PlayedGameState
{
	[Route("api/kf2/gamestate")]
	public class SaveGameStateController : Controller
	{
		public SaveGameStateController(
			IPlayedGameStore playedGameStore = null, 
			IPlayerStore playerStore = null,
			IPlayerPlayedWaveStore playerPlayedWaveStore = null,
			IPlayerPlayedGameStore playerPlayedGameStore = null,
			IPushNotificationSender pushNotificationSender = null,
			IMapStore mapStore = null,
			ILogger<SaveGameStateController> logger = null)
		{
			_playedGameStore = playedGameStore ?? new PlayedGameStore();
			_playerStore = playerStore ?? new PlayerStore();

			_playerPlayedWaveStore = playerPlayedWaveStore ?? new PlayerPlayedWaveStore();
			_playerPlayedGameStore = playerPlayedGameStore ?? new PlayerPlayedGameStore();

			_pushNotificationSender = pushNotificationSender ?? new PushNotificationSender();
			_mapStore = mapStore ?? new MapStore();

			_logger = logger ?? new LoggerFactory().CreateLogger<SaveGameStateController>();
		}

		[HttpPost("save")]
		public IActionResult HandleRequest([FromBody] GameState newGameState)
		{
			FixMapName(newGameState);
			RemoveOrFixInvalidPlayers(newGameState);

			_logger.LogDebug("Refreshing with Game State: {0}", JsonConvert.SerializeObject(newGameState));

			_playerStore.SaveAllPlayers(newGameState.Players);

			var currentPlayedGame = _playedGameStore.FindCurrentGame(newGameState.ServerId);

			if (currentPlayedGame == null && newGameState.CurrentWave == 1)
			{
				var details = new PushNotificationDetails
				{
					Title = $"SloshyDoshMan Inc",
					Content = $"New game has started on map {newGameState.Map} with {newGameState.Players.Count} players!",
					TypeName = "SloshyDoshManIncServerUpdate"
				};

				_logger.LogDebug(details.Content);
				_pushNotificationSender.NotifyAll(details);
				_playedGameStore.StartNewGame(newGameState);
				currentPlayedGame = _playedGameStore.FindCurrentGame(newGameState.ServerId);
			}

			if(currentPlayedGame != null)
			{
				if (newGameState.CurrentWave < currentPlayedGame.ReachedWave || newGameState.Map != currentPlayedGame.Map || newGameState.GameLength != currentPlayedGame.Length || newGameState.Difficulty != currentPlayedGame.Difficulty)
				{
					var reachedBossWave = currentPlayedGame.ReachedWave > currentPlayedGame.TotalWaves;
					var playersWon = reachedBossWave && FindPlayersWithLivingStatusForServer(newGameState.ServerId).Values.Any(isAlive => isAlive);
					_playedGameStore.EndGame(currentPlayedGame, playersWon);
					SteamIdsAliveInFinalWaveByServerIdMemoryCache.Remove(newGameState.ServerId);
				}
				else
				{
					_playedGameStore.UpdateGame(currentPlayedGame, newGameState);

					foreach (var player in newGameState.Players.Where(x => !string.IsNullOrEmpty(x.Perk)))
					{
						_playerPlayedGameStore.RecordPlayersPlayedGame(currentPlayedGame, player);
						_playerPlayedWaveStore.RecordPlayerPlayedWave(currentPlayedGame, player);
					}
				}
			}

			if(newGameState.CurrentWave > newGameState.TotalWaves)
			{
				var playersLivingStatusBySteamId = newGameState.Players.ToDictionary(x => x.SteamId, x => x.Health > 0);
				SteamIdsAliveInFinalWaveByServerIdMemoryCache.Set(newGameState.ServerId, FindPlayersWithLivingStatusForServer(newGameState.ServerId).Merge(playersLivingStatusBySteamId));
			}

			return Json(Result.Successful);
		}

		private void FixMapName(GameState newGameState)
		{
			if (_mapStore.FindMap(newGameState.Map, out var map))
			{
				newGameState.Map = map.Name;
			}
		}

		private void RemoveOrFixInvalidPlayers(GameState newGameState)
		{
			var existingPlayers = _playerStore.FindPlayersByName(newGameState.Players.Select(x => x.Name).ToList());
			newGameState.Players = newGameState.Players.GroupBy(x => x.Name).Select(x => x.First()).ToList();
		}

		private Dictionary<long, bool> FindPlayersWithLivingStatusForServer(Guid serverId) => SteamIdsAliveInFinalWaveByServerIdMemoryCache.GetOrCreate(serverId, (entry) => new Dictionary<long, bool>());

		private readonly IPlayerStore _playerStore;
		private readonly IPlayedGameStore _playedGameStore;

		private readonly IPlayerPlayedGameStore _playerPlayedGameStore;
		private readonly IPlayerPlayedWaveStore _playerPlayedWaveStore;

		private readonly IPushNotificationSender _pushNotificationSender;
		private readonly IMapStore _mapStore;
		private readonly ILogger<SaveGameStateController> _logger;

		private static IMemoryCache SteamIdsAliveInFinalWaveByServerIdMemoryCache { get; set; } = new MemoryCache(new MemoryCacheOptions());
	}
}
