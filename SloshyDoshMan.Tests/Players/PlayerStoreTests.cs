using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SloshyDoshMan.Service.Players;

namespace SloshyDoshMan.Tests.Players
{
	[TestFixture, DatabaseTests]
	public class PlayerStoreTests
	{
		[SetUp]
		public void SetUp()
		{
			_playerStore = new PlayerStore();
		}

		[Test]
		public void ShouldFindExistingPlayersByUsingExactNameInQuery()
		{
			GivenExistingPlayer(-1, "TestKF2PlayerName", "IPAdress");
			WhenFindPlayersWithQuery("TestKF2PlayerName");
			ThenPlayers.Should().HaveCount(1);
			ThenPlayers[0].Name.Should().Be("TestKF2PlayerName");
			ThenPlayers[0].LastKnownIP.Should().Be("IPAdress");
			ThenPlayers[0].SteamId.Should().Be(-1);
		}

		[Test]
		public void ShouldFindExistingPlayersByUsingPartialMatchOfNameInQuery()
		{
			GivenExistingPlayer(-1, "TestKF2PlayerName", "IPAdress");
			WhenFindPlayersWithQuery("KF2Player");
			ThenPlayers.Should().HaveCount(1);
			ThenPlayers[0].Name.Should().Be("TestKF2PlayerName");
			ThenPlayers[0].LastKnownIP.Should().Be("IPAdress");
			ThenPlayers[0].SteamId.Should().Be(-1);
		}

		[Test]
		public void ShouldNotFindPlayerWhenInvalidSteamIdIsReceived()
		{
			WhenTryFindPlayer(-1);
			ThenFoundPlayer.Should().BeFalse();
		}

		[Test]
		public void ShouldFindPlayerBySteamId()
		{
			GivenExistingPlayer(-1, "TestKF2PlayerName", "IPAdress");
			WhenTryFindPlayer(-1);
			ThenFoundPlayer.Should().BeTrue();
			ThenPlayer.Name.Should().Be("TestKF2PlayerName");
			ThenPlayer.LastKnownIP.Should().Be("IPAdress");
			ThenPlayer.SteamId.Should().Be(-1);
		}

		private void GivenExistingPlayer(long steamId, string name, string ipAddress)
		{
			_playerStore.SavePlayers(new[] { new Shared.PlayerGameState { SteamId = steamId, Name = name, IPAddress = ipAddress } });
		}

		private void WhenTryFindPlayer(long steamId)
		{
			ThenFoundPlayer = _playerStore.TryFindPlayer(steamId, out var player);
			ThenPlayer = player;
		}

		private void WhenFindPlayersWithQuery(string query)
		{
			ThenPlayers = _playerStore.Search(query);
		}

		private Player ThenPlayer { get; set; }
		private bool ThenFoundPlayer { get; set; }
		private IReadOnlyList<Player> ThenPlayers { get; set; }

		private PlayerStore _playerStore;
	}
}
