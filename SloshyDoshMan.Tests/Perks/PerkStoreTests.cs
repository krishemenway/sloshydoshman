using FluentAssertions;
using NUnit.Framework;
using SloshyDoshMan.Service.Perks;
using System.Collections.Generic;

namespace SloshyDoshMan.Tests.Perks
{
	[TestFixture]
	public class PerkStoreTests
	{
		[SetUp]
		public void SetUp()
		{
			_perkStore = new PerkStore();
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
