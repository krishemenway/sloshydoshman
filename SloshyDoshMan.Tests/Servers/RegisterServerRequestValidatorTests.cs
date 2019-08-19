using FluentAssertions;
using NUnit.Framework;
using SloshyDoshMan.Service.Servers;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Tests.Servers
{
	[TestFixture, UnitTests]
	public class RegisterServerRequestValidatorTests
	{
		[SetUp]
		public void SetUp()
		{
			GivenRequest = new RegisterServerRequest
			{
				KF2ServerIP = "127.0.0.1",
				ServerName = "ServerName",
			};

			_registerServerRequestValidator = new RegisterServerRequestValidator();
		}

		[Test]
		public void ShouldReturnFailureResultWhenServerNameExceedsMaximumLength()
		{
			GivenRequest.ServerName = new string('a', RegisterServerRequestValidator.MaxServerNameLength + 1);
			WhenValidating();
			ThenWasSuccessful.Should().BeFalse();
			ThenResult.Success.Should().BeFalse();
		}

		[Test]
		public void ShouldReturnSuccessResultWhenRequestIsSuccessful()
		{
			WhenValidating();
			ThenWasSuccessful.Should().BeTrue();
			ThenResult.Success.Should().BeTrue();
		}

		private void WhenValidating()
		{
			ThenWasSuccessful = _registerServerRequestValidator.Validate(GivenRequest, out var result);
			ThenResult = result;
		}

		private RegisterServerRequest GivenRequest { get; set; }
		private bool ThenWasSuccessful { get; set; }
		private Result ThenResult { get; set; }

		private RegisterServerRequestValidator _registerServerRequestValidator;
	}
}
