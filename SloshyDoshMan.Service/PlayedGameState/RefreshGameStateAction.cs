using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SloshyDoshMan.Service.Notifications;
using SloshyDoshMan.Service.PlayedGames;
using SloshyDoshMan.Service.Players;
using SloshyDoshMan.Shared;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.PlayedGameState
{
	public class RefreshGameStateAction
	{
		public RefreshGameStateAction(
			IPlayedGameStore playedGameStore = null, 
			IPlayerStore playerStore = null,
			IPlayerPlayedWaveStore playerPlayedWaveStore = null,
			IPlayerPlayedGameStore playerPlayedGameStore = null,
			IPushNotificationSender pushNotificationSender = null,
			ILogger<RefreshGameStateAction> logger = null)
		{
			_playedGameStore = playedGameStore ?? new PlayedGameStore();
			_playerStore = playerStore ?? new PlayerStore();

			_playerPlayedWaveStore = playerPlayedWaveStore ?? new PlayerPlayedWaveStore();
			_playerPlayedGameStore = playerPlayedGameStore ?? new PlayerPlayedGameStore();

			_pushNotificationSender = pushNotificationSender ?? new PushNotificationSender();
			_logger = logger ?? new LoggerFactory().CreateLogger<RefreshGameStateAction>();
		}

		public void RefreshGameState(GameState newGameState)
		{
			_logger.LogDebug("Refreshing with Game State: {0}", JsonConvert.SerializeObject(newGameState));

			RemoveOrFixInvalidPlayers(newGameState);
			_playerStore.SaveAllPlayers(newGameState.Players);

			var currentPlayedGame = _playedGameStore.FindCurrentGame();

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
				currentPlayedGame = _playedGameStore.FindCurrentGame();
			}

			if(currentPlayedGame != null)
			{
				if (newGameState.CurrentWave < currentPlayedGame.ReachedWave || newGameState.Map != currentPlayedGame.Map || newGameState.GameLength != currentPlayedGame.Length || newGameState.Difficulty != currentPlayedGame.Difficulty)
				{
					var reachedBossWave = currentPlayedGame.ReachedWave > currentPlayedGame.TotalWaves;
					var playersWon = reachedBossWave && SteamIdsAliveInFinalWave.Values.Any(isAlive => isAlive);
					_playedGameStore.EndGame(currentPlayedGame, playersWon);
					SteamIdsAliveInFinalWave.Clear();
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
				SteamIdsAliveInFinalWave.Merge(newGameState.Players.ToDictionary(x => x.SteamId, x => x.Health > 0));
				LastGameState = newGameState;
			}
		}
		
		private void RemoveOrFixInvalidPlayers(GameState newGameState)
		{
			var existingPlayers = _playerStore.FindPlayersByName(newGameState.Players.Select(x => x.Name).ToList());
			newGameState.Players = newGameState.Players.GroupBy(x => x.Name).Select(x => x.First()).ToList();
		}

		private readonly IPlayerStore _playerStore;
		private readonly IPlayedGameStore _playedGameStore;

		private readonly IPlayerPlayedGameStore _playerPlayedGameStore;
		private readonly IPlayerPlayedWaveStore _playerPlayedWaveStore;

		private readonly IPushNotificationSender _pushNotificationSender;
		private readonly ILogger<RefreshGameStateAction> _logger;

		private static IGameState LastGameState { get; set; }
		private static Dictionary<long, bool> SteamIdsAliveInFinalWave { get; set; } = new Dictionary<long, bool>();

		private readonly ILogger<RefreshGameStateAction> _logger;
	}
}
