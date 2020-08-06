using FluentAssertions;
using Moq;
using NUnit.Framework;
using SloshyDoshMan.Service.Players;
using SloshyDoshMan.Shared;
using System.Collections.Generic;

namespace SloshyDoshMan.Tests.Players
{
	[TestFixture, UnitTests]
	public class PlayerSearchControllerTests
	{
		[SetUp]
		public void SetUp()
		{
			GivenRequest = new PlayerSearchRequest
			{
				Query = "Query",
			};

			GivenPlayersFromSearch = new List<Player>
			{
				new Player { SteamId = "-1", Name = "PlayerName" },
			};

			_playerStore = new Mock<IPlayerStore>();
			_playerStore.Setup(x => x.Search(GivenRequest.Query)).Returns(GivenPlayersFromSearch);

			_playerSearchController = new PlayerSearchController(_playerStore.Object);
		}

		[Test]
		public void ShouldReturnSuccessResultContainingSearchResults()
		{
			WhenSearching();
			ThenResult.Success.Should().BeTrue();
			ThenResult.Data[0].SteamId.Should().Be(GivenPlayersFromSearch[0].SteamId);
			ThenResult.Data[0].UserName.Should().Be(GivenPlayersFromSearch[0].Name.ToString());
			ThenResult.Data[0].MapStatistics.Should().BeEmpty();
			ThenResult.Data[0].PerkStatistics.Should().BeEmpty();
		}

		private void WhenSearching()
		{
			ThenResult = _playerSearchController.Search(GivenRequest).Value;
		}

		private List<Player> GivenPlayersFromSearch { get; set; }
		private PlayerSearchRequest GivenRequest { get; set; }

		private Result<IReadOnlyList<PlayerProfile>> ThenResult { get; set; }

		private Mock<IPlayerStore> _playerStore;

		private PlayerSearchController _playerSearchController;
	}
}
