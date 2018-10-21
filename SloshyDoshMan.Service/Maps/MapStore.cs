using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SloshyDoshMan.Service.Maps
{
	public interface IMapStore
	{
		bool FindMap(string mapName, out Map map);
		IReadOnlyList<Map> FindAllMaps();
	}

	public class MapStore : IMapStore
	{
		public IReadOnlyList<Map> FindAllMaps()
		{
			return LazyMapList.Value.Select(x => x.Value).ToList();
		}

		public bool FindMap(string mapName, out Map map)
		{
			return LazyMapList.Value.TryGetValue(mapName.ToLower(), out map);
		}

		private static IReadOnlyDictionary<string, Map> LoadMapList()
		{
			var stream = Assembly.GetEntryAssembly().GetManifestResourceStream("SloshyDoshMan.Service.Maps.Maps.json");

			using (var streamReader = new StreamReader(stream))
			{
				return JsonConvert
					.DeserializeObject<IReadOnlyList<Map>>(streamReader.ReadToEnd())
					.ToDictionary(map => map.Name.ToLower(), map => map);
			}
		}

		public static Lazy<IReadOnlyDictionary<string, Map>> LazyMapList = new Lazy<IReadOnlyDictionary<string, Map>>(LoadMapList);
	}
}
