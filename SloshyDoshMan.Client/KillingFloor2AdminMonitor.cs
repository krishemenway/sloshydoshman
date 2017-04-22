﻿using SloshyDoshMan.Shared;
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
			ISloshyDoshManStatsService sloshyDoshManStatsService = null)
		{
			_killingFloor2AdminClient = killingFloor2AdminClient ?? new KillingFloor2AdminClient();
			_killingFloor2AdminScraper = killingFloor2AdminScraper ?? new KillingFloor2AdminScraper();
			_sloshyDoshManStatsService = sloshyDoshManStatsService ?? new SloshyDoshManStatsService();
		}

		public void StartMonitoring()
		{
			RefreshStateTimer = new Timer(Settings.Instance.RefreshIntervalInMilliseconds);
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

				if(oldGameState != null && ShouldSendAdvertisement(oldGameState, CurrentGameState))
				{
					_killingFloor2AdminClient.SendMessage(Settings.Instance.AdvertisementMessage);
				}

				if(oldGameState == null || GameStateHasChanged(oldGameState, CurrentGameState))
				{
					_sloshyDoshManStatsService.SaveGameState(CurrentGameState);
				}
			}
			catch(AggregateException aggregateException)
			{
				aggregateException.InnerExceptions
					.Select(x => x.Message).ToList()
					.ForEach(Console.WriteLine);
			}
			catch(Exception exception)
			{
				Console.WriteLine(exception.Message);
			}
			
			RefreshStateTimer.Start();
		}

		public void StopMonitoring()
		{
			if(RefreshStateTimer != null)
			{
				RefreshStateTimer.Stop();
				RefreshStateTimer.Dispose();
			}
		}

		private static bool GameStateHasChanged(GameState oldState, GameState newState)
		{
			if(oldState.CurrentWave != newState.CurrentWave || oldState.Difficulty != newState.Difficulty || oldState.GameLength != newState.GameLength || oldState.Map != newState.Map)
			{
				return true;
			}

			if(oldState.Players.Count != newState.Players.Count)
			{
				return true;
			}

			var matchingPlayerCount = oldState.Players.Select(x => x.SteamId).Intersect(newState.Players.Select(x => x.SteamId)).Count();

			if(matchingPlayerCount != oldState.Players.Count || matchingPlayerCount != newState.Players.Count)
			{
				return true;
			}
			
			foreach(var player in oldState.Players)
			{
				var newStatePlayer = newState.Players.FirstOrDefault(x => x.SteamId == player.SteamId);

				if(newStatePlayer.Kills != player.Kills || newStatePlayer.Perk != player.Perk)
				{
					return true;
				}
			}

			return false;
		}

		private static bool ShouldSendAdvertisement(GameState oldGameState, GameState newGameState)
		{
			return Settings.Instance.EnableAdvertisement && oldGameState.CurrentWave < newGameState.CurrentWave && newGameState.CurrentWave % 2 == 0;
		}

		private GameState CurrentGameState { get; set; }
		private Timer RefreshStateTimer { get; set; }

		private readonly IKillingFloor2AdminScraper _killingFloor2AdminScraper;
		private readonly ISloshyDoshManStatsService _sloshyDoshManStatsService;
		private IKillingFloor2AdminClient _killingFloor2AdminClient;
	}
}
