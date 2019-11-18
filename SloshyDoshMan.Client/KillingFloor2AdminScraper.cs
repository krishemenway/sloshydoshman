using AngleSharp.Dom;
using SloshyDoshMan.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Client
{
	public interface IKillingFloor2AdminScraper
	{
		GameState GetCurrentGameState();
		string FindServerName();
	}

	public class KillingFloor2AdminScraper : IKillingFloor2AdminScraper
	{
		public KillingFloor2AdminScraper(IKillingFloor2AdminClient killingFloor2AdminClient = null)
		{
			_killingFloor2AdminClient = killingFloor2AdminClient ?? new KillingFloor2AdminClient();
		}

		public string FindServerName()
		{
			var scoreboardContent = _killingFloor2AdminClient.GetScoreboardContent();
			var currentGameElements = scoreboardContent.GetElementById("currentGame").Children;
			return currentGameElements[1].TextContent;
		}

		public GameState GetCurrentGameState()
		{
			if (!Program.ServerId.HasValue)
			{
				throw new Exception("No ServerId received from server. Make sure you can reach the SloshyDoshMan service API from this client.");
			}

			var playerMetadataContent = _killingFloor2AdminClient.GetPlayersContent();
			var scoreboardContent = _killingFloor2AdminClient.GetScoreboardContent();

			if(scoreboardContent.GetElementById("loginform") != null || playerMetadataContent.GetElementById("loginform") != null)
			{
				throw new InvalidOperationException("Cannot connect to KF2 Admin because Http Auth is not enabled. Make sure that bHttpAuth=true in KFWebAdmin.ini");
			}

			try
			{
				var currentGameElements = scoreboardContent.GetElementById("currentGame").Children;
				var currentRulesElements = scoreboardContent.GetElementById("currentRules").Children;
				var waveParts = currentRulesElements[1].TextContent.Split('/');

				return new GameState
				{
					ServerId = Program.ServerId.Value,
					ServerName = currentGameElements[1].TextContent,
					Map = currentGameElements[7].Attributes["title"].Value,
					Difficulty = currentRulesElements[3].TextContent,
					GameType = currentGameElements[5].TextContent,

					CurrentWave = Convert.ToInt32(waveParts[0]),
					TotalWaves = Convert.ToInt32(waveParts[1]),

					Players = ParsePlayerData(scoreboardContent, playerMetadataContent)
				};
			}
			catch (Exception)
			{
				throw new Exception("Could not parse response from KF2 Admin server. Was there a KF2 patch recently? Did something change?");
			}
		}

		private static List<PlayerGameState> ParsePlayerData(IDocument scoreboardContent, IDocument playerMetadataContent)
		{
			var allPlayerMetadata = ParsePlayerMetadata(playerMetadataContent);
			var allPlayers = scoreboardContent.QuerySelectorAll("#players tbody tr");

			var playerGameStates = new List<PlayerGameState>();

			foreach(var playerStateElement in allPlayers)
			{
				var playerStateElements = playerStateElement.Children;

				if(playerStateElements.Count() == 1) // the only element is informing "There are no players"
				{
					continue;
				}
				
				var name = playerStateElements[1].TextContent;
				var matchedPlayerMetadata = allPlayerMetadata.FirstOrDefault(x => x.Name == name);
				
				if(matchedPlayerMetadata != null)
				{
					var playerGameState = new PlayerGameState
					{
						SteamId = matchedPlayerMetadata.SteamID,
						Name = name,

						IPAddress = matchedPlayerMetadata.IPAddress,
						UniqueNetId = matchedPlayerMetadata.UniqueNetId,

						Perk = playerStateElements[2].TextContent,
						Dosh = Convert.ToInt32(playerStateElements[3].TextContent),
						Kills = Convert.ToInt32(playerStateElements[5].TextContent),
						Ping = Convert.ToInt32(playerStateElements[6].TextContent)
					};

					if(!string.IsNullOrWhiteSpace(playerStateElements[4].TextContent))
					{
						playerGameState.Health = Convert.ToInt32(playerStateElements[4].TextContent);
					}

					playerGameStates.Add(playerGameState);
				}
			}
			
			return playerGameStates;
		}

		private static List<PlayersListData> ParsePlayerMetadata(IDocument playerListContent)
		{
			var playerListData = new List<PlayersListData>();

			foreach(var playerListItemWrapper in playerListContent.QuerySelectorAll("#players tbody tr"))
			{
				var playerListItems = playerListItemWrapper.Children;

				if (playerListItems.Count() == 1)
				{
					continue;
				}

				var player = new PlayersListData
				{
					Name = playerListItems[1].TextContent,
					IPAddress = playerListItems[3].TextContent,
					UniqueNetId = playerListItems[4].TextContent,
					SteamID = Convert.ToInt64(playerListItems[5].TextContent),
				};

				playerListData.Add(player);
			}

			return playerListData;
		}

		private readonly IKillingFloor2AdminClient _killingFloor2AdminClient;
	}

	public class PlayersListData
	{
		public long SteamID { get; set; }
		public string Name { get; set; }
		public string IPAddress { get; set; }
		public string UniqueNetId { get; set; }
	}
}
