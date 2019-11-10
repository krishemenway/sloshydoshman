using FluentAssertions;
using Moq;
using NUnit.Framework;
using SloshyDoshMan.Service.Servers;
using SloshyDoshMan.Shared;
using System;
using System.Net;

namespace SloshyDoshMan.Tests.Servers
{
	[TestFixture, UnitTests]
	public class RegisterServerControllerTests
	{
		[SetUp]
		public void SetUp()
		{
			GivenRequest = new RegisterServerRequest
			{
				ServerName = "ServerName",
				KF2ServerIP = "IPAddress",
			};

			GivenCreatedServer = new Server
			{
				ServerId = Guid.NewGuid(),
				CurrentName = GivenRequest.ServerName,
				LastKnownIPAddress = GivenRequestIPAddress.ToString(),
			};

			_serverStore = new Mock<IServerStore>();
			_serverStore.Setup(x => x.CreateNewServer(GivenRequest.ServerName, GivenRequestIPAddress.ToString())).Returns(GivenCreatedServer);

			_registerServerRequestValidator = new Mock<IRegisterServerRequestValidator>();

			_findRemoteIPAddressFunc = () => GivenRequestIPAddress.ToString();

			_registerServerController = new RegisterServerController(_serverStore.Object, _registerServerRequestValidator.Object, _findRemoteIPAddressFunc);
		}

		[Test]
		public void ShouldReturnFailureResultWhenValidatorFails()
		{
			GivenValidationIsSuccessful(false);
			WhenRegisteringServer();
			ThenResult.Success.Should().BeFalse();
			ThenResult.ErrorMessage.Should().Be(ValidationFailureMessage);
		}

		[Test]
		public void ShouldReturnSuccessResultWhenValidatorSucceeds()
		{
			GivenValidationIsSuccessful();
			WhenRegisteringServer();
			ThenResult.Success.Should().BeTrue();
			ThenResult.Data.ServerId.Should().Be(GivenCreatedServer.ServerId);
			ThenResult.Data.LastKnownIPAddress.Should().Be(GivenRequestIPAddress.ToString());
		}

		private void GivenValidationIsSuccessful(bool isSuccessful = true)
		{
			var result = isSuccessful ? Result.Successful : Result.Failure(ValidationFailureMessage);
			_registerServerRequestValidator.Setup(x => x.Validate(GivenRequest, out result)).Returns(result.Success);
		}

		private void WhenRegisteringServer()
		{
			ThenResult = _registerServerController.Register(GivenRequest).Value;
		}

		private string ValidationFailureMessage { get; } = "FailureMessage";
		private IPAddress GivenRequestIPAddress { get; } = new IPAddress(new byte[] { 192, 168, 0, 1 });

		private RegisterServerRequest GivenRequest { get; set; }
		private Server GivenCreatedServer { get; set; }
		private Result<IServer> ThenResult { get; set; }

		private Mock<IServerStore> _serverStore;
		private Mock<IRegisterServerRequestValidator> _registerServerRequestValidator;
		private Func<string> _findRemoteIPAddressFunc;
		private RegisterServerController _registerServerController;
	}
}
