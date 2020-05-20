using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SloshyDoshMan.Service.Perks;
using System;
using System.Collections.Generic;
using System.IO;

namespace SloshyDoshMan.Tests.Perks
{
	[TestFixture]
	public class PerkStoreTests
	{
		[SetUp]
		public void SetUp()
		{
			var configuration = new Lazy<IConfigurationRoot>(() => new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("perks.json").Build());
			_perkStore = new PerkStore(configuration);
		}

		[Test]
		public void ShouldLoadPerksFromFile()
		{
			WhenLoadingAllPerks();
			ThenPerks.Count.Should().BeGreaterThan(0);
			ThenPerks.Should().OnlyContain(x => !string.IsNullOrEmpty(x.Name));
		}

		private void WhenLoadingAllPerks()
		{
			ThenPerks = _perkStore.FindAllPerks();
		}

		private IReadOnlyList<Perk> ThenPerks { get; set; }

		private PerkStore _perkStore;
	}
}
