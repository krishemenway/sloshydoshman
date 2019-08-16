using FluentAssertions;
using NUnit.Framework;
using SloshyDoshMan.Service.Servers;
using SloshyDoshMan.Shared;
using System;

namespace SloshyDoshMan.Tests.Servers
{
	[TestFixture, DatabaseTests]
	public class ServerStoreTests
	{
		[SetUp]
		public void SetUp()
		{
			GivenServerName = "Server Name";
			GivenIPAddress = "IP Address";

			_serverStore = new ServerStore();
		}

		[Test]
		public void ShouldNotFindServerThatDoesNotExist()
		{
			WhenFindingServer(Guid.Empty);
			ThenServerWasFound.Should().BeFalse();
		}

		[Test]
		public void ShouldCreateServerForNameAndIPAddress()
		{
			WhenCreatingServer();
			ThenCreatedServer.ServerId.Should().NotBeEmpty();
			ThenCreatedServer.CurrentName.Should().Be(GivenServerName);
			ThenCreatedServer.LastKnownIPAddress.Should().Be(GivenIPAddress);

			WhenFindingServer(ThenCreatedServer.ServerId);
			ThenServerWasFound.Should().BeTrue();
			ThenFoundServer.ServerId.Should().Be(ThenCreatedServer.ServerId);
			ThenFoundServer.LastKnownIPAddress.Should().Be(ThenCreatedServer.LastKnownIPAddress);
			ThenFoundServer.CurrentName.Should().Be(ThenCreatedServer.CurrentName);
		}

		private void WhenCreatingServer()
		{
			ThenCreatedServer = _serverStore.CreateNewServer(GivenServerName, GivenIPAddress);
		}

		private void WhenFindingServer(Guid serverId)
		{
			ThenServerWasFound = _serverStore.TryFindServer(serverId, out var server);
			ThenFoundServer = server;
		}

		private string GivenServerName { get; set; }
		private string GivenIPAddress { get; set; }
		private IServer ThenCreatedServer { get; set; }
		public bool ThenServerWasFound { get; private set; }
		public IServer ThenFoundServer { get; private set; }

		private IServerStore _serverStore;
	}
}
