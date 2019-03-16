using Newtonsoft.Json;
using SloshyDoshMan.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SloshyDoshMan.Service.Maps
{
	public interface IMapStore
	{
		IReadOnlyList<IMap> FindAllMaps();
		IReadOnlyList<IMap> FindCoreMaps();
		IReadOnlyList<(string MapName, Difficulty Difficulty)> FindCoreMapDifficulties();

		bool FindMap(string mapName, out IMap map);
	}

	public class MapStore : IMapStore
	{
		public IReadOnlyList<IMap> FindAllMaps()
		{
			return LazyMapList.Value.Values.ToList();
		}

		public IReadOnlyList<IMap> FindCoreMaps()
		{
			return LazyCoreMaps.Value.Values.ToList();
		}

		public IReadOnlyList<(string MapName, Difficulty Difficulty)> FindCoreMapDifficulties()
		{
			return LazyCoreMapDifficulties.Value;
		}

		public bool FindMap(string mapName, out IMap map)
		{
			return LazyMapList.Value.TryGetValue(mapName.ToLower(), out map);
		}

		private static IReadOnlyDictionary<string, IMap> LoadMapList()
		{
			using (var streamReader = new StreamReader(MapsResourceStream))
			{
				return JsonConvert
					.DeserializeObject<IReadOnlyList<Map>>(streamReader.ReadToEnd())
					.ToDictionary(map => map.Name.ToLower(), map => map as IMap);
			}
		}

		private static IReadOnlyDictionary<string, IMap> LoadCoreMaps()
		{
			return LazyMapList.Value.Values
				.Where(map => !map.IsWorkshop)
				.ToDictionary(map => map.Name, map => map);
		}

		private static IReadOnlyList<(string MapName, Difficulty Difficulty)> LoadCoreMapDifficulties()
		{
			var allDifficulties = Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>();

			return LazyCoreMaps.Value.Values
				.SelectMany(map => allDifficulties.Select(difficulty => (MapName: map.Name, Difficulty: difficulty)))
				.ToList();
		}

		private static Stream MapsResourceStream => Assembly.GetEntryAssembly().GetManifestResourceStream("SloshyDoshMan.Service.Maps.Maps.json");

		private static Lazy<IReadOnlyDictionary<string, IMap>> LazyMapList = new Lazy<IReadOnlyDictionary<string, IMap>>(LoadMapList);
		private static Lazy<IReadOnlyDictionary<string, IMap>> LazyCoreMaps = new Lazy<IReadOnlyDictionary<string, IMap>>(LoadCoreMaps);
		private static Lazy<IReadOnlyList<(string MapName, Difficulty Difficulty)>> LazyCoreMapDifficulties = new Lazy<IReadOnlyList<(string MapName, Difficulty Difficulty)>>(LoadCoreMapDifficulties);
	}
}
