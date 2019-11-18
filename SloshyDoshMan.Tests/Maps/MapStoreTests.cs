using FluentAssertions;
using NUnit.Framework;
using SloshyDoshMan.Service.Maps;
using SloshyDoshMan.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Tests.Maps
{
	[TestFixture]
	public class MapStoreTests
	{
		[SetUp]
		public void SetUp()
		{
			_mapStore = new MapStore();
		}

		[Test]
		public void ShouldFindAllMaps()
		{
			WhenFindingAllMaps();
			ThenMaps.Should().NotBeEmpty();
		}

		[Test]
		public void ShouldFindAllCoreMaps()
		{
			WhenFindingAllCoreMaps();
			ThenMaps.Should().NotBeEmpty();
			ThenMaps.Should().OnlyContain(x => !x.IsWorkshop);
		}

		[Test]
		public void ShouldFindAllCoreMapDifficulties()
		{
			WhenFindingAllCoreMaps();
			WhenFindingAllCoreMapDifficulties();

			ThenMapDifficulties
				.GroupBy(x => x.MapName)
				.Should().OnlyContain(x => x.Count() == Enum.GetValues(typeof(Difficulty)).Length);

			ThenMapDifficulties.Should().OnlyContain(mapDifficulty => ThenMaps.Any(m => m.Name == mapDifficulty.MapName));
		}

		[Test]
		public void ShouldNotFindMapThatDoesNotExist()
		{
			WhenTryFindMap("Random MapName that hopefully never exists");
			ThenFoundMap.Should().BeFalse();
		}

		[Test]
		public void ShouldFindMapThatExists()
		{
			WhenTryFindMap("KF-Outpost");
			ThenFoundMap.Should().BeTrue();
			ThenMap.Name.Should().Be("KF-Outpost");
			ThenMap.IsWorkshop.Should().BeFalse();
		}

		private void WhenTryFindMap(string mapName)
		{
			ThenFoundMap = _mapStore.TryFindMap(mapName, out var map);
			ThenMap = map;
		}

		private void WhenFindingAllCoreMapDifficulties()
		{
			ThenMapDifficulties = _mapStore.FindCoreMapDifficulties();
		}

		private void WhenFindingAllCoreMaps()
		{
			ThenMaps = _mapStore.FindCoreMaps();
		}

		private void WhenFindingAllMaps()
		{
			ThenMaps = _mapStore.FindAllMaps();
		}

		private IReadOnlyList<IMap> ThenMaps { get; set; }
		private IReadOnlyList<(string MapName, Difficulty Difficulty)> ThenMapDifficulties { get; set; }
		private bool ThenFoundMap { get; set; }
		private IMap ThenMap { get; set; }

		private MapStore _mapStore;
	}
}
