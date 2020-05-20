using Serilog;
using SloshyDoshMan.Shared;
using System;
using System.Linq;
using System.Timers;

namespace SloshyDoshMan.Client
{
	public interface IKillingFloor2AdminMonitor
	{
		void StartMonitoring();
		void StopMonitoring();
	}

	public class KillingFloor2AdminMonitor : IKillingFloor2AdminMonitor
	{
		public KillingFloor2AdminMonitor(
			IKillingFloor2AdminScraper killingFloor2AdminScraper = null,
			IKillingFloor2AdminClient killingFloor2AdminClient = null,
			ISloshyDoshManStatsService sloshyDoshManService = null,
			ISettings settings = null)
		{
			_killingFloor2AdminClient = killingFloor2AdminClient ?? new KillingFloor2AdminClient();
			_killingFloor2AdminScraper = killingFloor2AdminScraper ?? new KillingFloor2AdminScraper();
			_sloshyDoshManService = sloshyDoshManService ?? new SloshyDoshManService();
			_settings = settings ?? Program.Settings;
		}

		public void StartMonitoring()
		{
			RefreshStateTimer = new Timer(Program.Settings.RefreshIntervalInMilliseconds);
			RefreshStateTimer.Elapsed += RefreshStateTimer_Elapsed;
			RefreshStateTimer.Start();
		}

		private void RefreshStateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			RefreshStateTimer.Stop();

			try
			{
				var oldGameState = CurrentGameState;
				CurrentGameState = _killingFloor2AdminScraper.GetCurrentGameState();

				Log.Debug("Received Game State: {@CurrentGameState}", CurrentGameState);

				if (oldGameState != null && ShouldSendAdvertisement(oldGameState, CurrentGameState))
				{
					_killingFloor2AdminClient.SendMessage(Program.Settings.AdvertisementMessage);
				}

				if (oldGameState == null || GameStateHasChanged(oldGameState, CurrentGameState))
				{
					_sloshyDoshManService.SaveGameState(CurrentGameState);
					LastSavedGameStateTime = DateTimeOffset.Now;
				}

				if (!string.IsNullOrEmpty(_settings.PreferMap) && DateTimeOffset.Now - LastSavedGameStateTime > TimeSpan.FromHours(1))
				{
					_killingFloor2AdminClient.ChangeMap(_settings.PreferMap);
				}
			}
			catch (AggregateException aggregateException)
			{
				aggregateException.InnerExceptions
					.Select(x => x.Message).ToList()
					.ForEach((exception) => { Log.Error(exception, "Something went wrong refreshing game state"); });
			}
			catch (Exception exception)
			{
				Log.Error(exception, "Something went wrong refreshing game state");
			}

			RefreshStateTimer.Start();
		}

		public void StopMonitoring()
		{
			if (RefreshStateTimer != null)
			{
				RefreshStateTimer.Stop();
				RefreshStateTimer.Dispose();
			}
		}

		private static bool GameStateHasChanged(GameState oldState, GameState newState)
		{
			if (oldState.CurrentWave != newState.CurrentWave || oldState.Difficulty != newState.Difficulty || oldState.GameLength != newState.GameLength || oldState.Map != newState.Map)
			{
				return true;
			}

			if (oldState.Players.Count != newState.Players.Count)
			{
				return true;
			}

			var matchingPlayerCount = oldState.Players.Select(x => x.SteamId).Intersect(newState.Players.Select(x => x.SteamId)).Count();

			if (matchingPlayerCount != oldState.Players.Count || matchingPlayerCount != newState.Players.Count)
			{
				return true;
			}

			foreach (var player in oldState.Players)
			{
				var newStatePlayer = newState.Players.FirstOrDefault(x => x.SteamId == player.SteamId);

				if (newStatePlayer.Kills != player.Kills || newStatePlayer.Perk != player.Perk)
				{
					return true;
				}
			}

			return false;
		}

		private bool ShouldSendAdvertisement(GameState oldGameState, GameState newGameState)
		{
			return Program.Settings.EnableAdvertisement && oldGameState.CurrentWave < newGameState.CurrentWave && newGameState.CurrentWave.IsEven();
		}

		private GameState CurrentGameState { get; set; }
		private Timer RefreshStateTimer { get; set; }
		private DateTimeOffset LastSavedGameStateTime { get; set; }

		private readonly IKillingFloor2AdminScraper _killingFloor2AdminScraper;
		private readonly ISloshyDoshManStatsService _sloshyDoshManService;
		private readonly IKillingFloor2AdminClient _killingFloor2AdminClient;
		private readonly ISettings _settings;
	}

	public static class IntExtensions
	{
		public static bool IsEven(this int value)
		{
			return value % 2 == 0;
		}
	}
}
