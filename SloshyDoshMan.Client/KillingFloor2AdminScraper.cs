using CsQuery;
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
			var scoreboardContent = CQ.Create(_killingFloor2AdminClient.GetScoreboardContent());
			var currentGameElements = scoreboardContent.Select("#currentGame").Children();
			return currentGameElements[1].InnerText;
		}

		public GameState GetCurrentGameState()
		{
			if (!Program.ServerId.HasValue)
			{
				throw new Exception("No ServerId received from server. Make sure you can reach the SloshyDoshMan service API from this client.");
			}

			var playerMetadataContent = CQ.Create(_killingFloor2AdminClient.GetPlayersContent());
			var scoreboardContent = CQ.Create(_killingFloor2AdminClient.GetScoreboardContent());

			if(scoreboardContent.Select("#loginform").Count() > 0 || playerMetadataContent.Select("#loginform").Count() > 0)
			{
				throw new InvalidOperationException("Cannot connect to KF2 Admin because Http Auth is not enabled. Make sure that bHttpAuth=true in KFWebAdmin.ini");
			}

			try
			{
				var currentGameElements = scoreboardContent.Select("#currentGame").Children();
				var currentRulesElements = scoreboardContent.Select("#currentRules").Children();
				var waveParts = currentRulesElements[1].InnerText.Split('/');

				return new GameState
				{
					ServerId = Program.ServerId.Value,
					ServerName = currentGameElements[1].InnerText,
					Map = currentGameElements[7].Attributes["title"],
					Difficulty = currentRulesElements[3].InnerText,
					GameType = currentGameElements[5].InnerText,

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

		private static List<PlayerGameState> ParsePlayerData(CQ scoreboardContent, CQ playerMetadataContent)
		{
			var allPlayerMetadata = ParsePlayerMetadata(playerMetadataContent);
			var allPlayers = scoreboardContent.Select("#players tbody tr");

			var playerGameStates = new List<PlayerGameState>();

			foreach(var playerStateElement in allPlayers)
			{
				var playerStateElements = playerStateElement.ChildElements.ToList();

				if(playerStateElements.Count == 1) // the only element is informing "There are no players"
				{
					continue;
				}
				
				var name = playerStateElements[1].InnerText;
				var matchedPlayerMetadata = allPlayerMetadata.FirstOrDefault(x => x.Name == name);
				
				if(matchedPlayerMetadata != null)
				{
					var playerGameState = new PlayerGameState
					{
						SteamId = matchedPlayerMetadata.SteamID,
						Name = name,

						IPAddress = matchedPlayerMetadata.IPAddress,
						UniqueNetId = matchedPlayerMetadata.UniqueNetId,

						Perk = playerStateElements[2].InnerText,
						Dosh = Convert.ToInt32(playerStateElements[3].InnerText),
						Kills = Convert.ToInt32(playerStateElements[5].InnerText),
						Ping = Convert.ToInt32(playerStateElements[6].InnerText)
					};

					if(!string.IsNullOrWhiteSpace(playerStateElements[4].InnerText))
					{
						playerGameState.Health = Convert.ToInt32(playerStateElements[4].InnerText);
					}

					playerGameStates.Add(playerGameState);
				}
			}
			
			return playerGameStates;
		}

		private static List<PlayersListData> ParsePlayerMetadata(CQ playerListContent)
		{
			var playerListData = new List<PlayersListData>();

			foreach(var playerListItemWrapper in playerListContent.Select("#players tbody tr"))
			{
				var playerListItems = playerListItemWrapper.ChildElements.ToList();

				if (playerListItems.Count == 1)
				{
					continue;
				}

				var player = new PlayersListData
				{
					Name = playerListItems[1].InnerText,
					IPAddress = playerListItems[3].InnerText,
					UniqueNetId = playerListItems[4].InnerText,
					SteamID = Convert.ToInt64(playerListItems[5].InnerText),
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
