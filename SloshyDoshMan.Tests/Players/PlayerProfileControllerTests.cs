using FluentAssertions;
using Moq;
using NUnit.Framework;
using SloshyDoshMan.Service.PlayedGames;
using SloshyDoshMan.Service.Players;
using SloshyDoshMan.Shared;
using System.Collections.Generic;

namespace SloshyDoshMan.Tests.Players
{
	[TestFixture, UnitTests]
	public class PlayerProfileControllerTests
	{
		[SetUp]
		public void SetUp()
		{
			GivenRequest = new PlayerProfileRequest
			{
				SteamId = "-1",
			};

			GivenPlayer = new Player
			{
				Name = "PlayerName",
				SteamId = GivenRequest.SteamId,
			};

			GivenAllGames = new List<IPlayedGame>
			{
				new PlayedGame(),
			};

			GivenMapStatistics = new List<PlayerMapStatistic>()
			{
				new PlayerMapStatistic(),
			};

			GivenPerkStatistics = new List<PlayerPerkStatistic>()
			{
				new PlayerPerkStatistic(),
			};

			_playerStatisticsStore = new Mock<IPlayerStatisticsStore>();
			_playerStatisticsStore.Setup(x => x.FindMapStatistics(GivenRequest.SteamId)).Returns(GivenMapStatistics);
			_playerStatisticsStore.Setup(x => x.FindPerkStatistics(GivenRequest.SteamId)).Returns(GivenPerkStatistics);

			_playedGameStore = new Mock<IPlayedGameStore>();
			_playedGameStore.Setup(x => x.FindAllGames(GivenRequest.SteamId)).Returns(GivenAllGames);

			_playerStore = new Mock<IPlayerStore>();

			_playerProfileController = new PlayerProfileController(_playerStatisticsStore.Object, _playedGameStore.Object, _playerStore.Object);
		}

		[Test]
		public void ShouldReturnFailureResultWhenRequestingSteamIdThatDoesNotExist()
		{
			GivenPlayerExists(false);
			WhenRequestingProfile();
			ThenResult.Success.Should().BeFalse();
			ThenResult.ErrorMessage.Should().Be("Unable to find profile for steam id");
		}

		[Test]
		public void ShouldReturnSuccessResultWhenRequestingSteamIdThatDoesExist()
		{
			GivenPlayerExists();
			WhenRequestingProfile();
			ThenResult.Success.Should().BeTrue();
			ThenResult.Data.SteamId.Should().Be(GivenRequest.SteamId);
			ThenResult.Data.UserName.Should().Be(GivenPlayer.Name);
			ThenResult.Data.AllGames.Should().BeEquivalentTo(GivenAllGames);
			ThenResult.Data.MapStatistics.Should().BeEquivalentTo(GivenMapStatistics);
			ThenResult.Data.PerkStatistics.Should().BeEquivalentTo(GivenPerkStatistics);
		}

		private void GivenPlayerExists(bool exists = true)
		{
			var player = GivenPlayer;
			_playerStore.Setup(x => x.TryFindPlayer(GivenRequest.SteamId, out player)).Returns(exists);
		}

		private void WhenRequestingProfile()
		{
			ThenResult = _playerProfileController.Profile(GivenRequest).Value;
		}

		private PlayerProfileRequest GivenRequest { get; set; }
		private Player GivenPlayer { get; set; }
		private List<PlayerMapStatistic> GivenMapStatistics { get; set; }
		private List<PlayerPerkStatistic> GivenPerkStatistics { get; set; }
		private Result<PlayerProfile> ThenResult { get; set; }
		private List<IPlayedGame> GivenAllGames { get; set; }

		private Mock<IPlayerStatisticsStore> _playerStatisticsStore;
		private Mock<IPlayedGameStore> _playedGameStore;
		private Mock<IPlayerStore> _playerStore;

		private PlayerProfileController _playerProfileController;
	}
}
