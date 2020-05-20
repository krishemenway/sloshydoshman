using Microsoft.Extensions.Configuration;
using SloshyDoshMan.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.Maps
{
	public interface IMapStore
	{
		IReadOnlyList<IMap> FindAllMaps();
		IReadOnlyList<IMap> FindCoreMaps();
		IReadOnlyList<(string MapName, Difficulty Difficulty)> FindCoreMapDifficulties();

		bool TryFindMap(string mapName, out IMap map);
	}

	public class MapStore : IMapStore
	{
		public MapStore(Lazy<IConfigurationRoot> configuration = null)
		{
			_configuration = configuration ?? LazyConfiguration;
		}

		public IReadOnlyList<IMap> FindAllMaps()
		{
			return _configuration.Value.Get<MapConfiguration>().Maps;
		}

		public IReadOnlyList<IMap> FindCoreMaps()
		{
			return FindAllMaps().Where(map => !map.IsWorkshop).ToList();
		}

		public IReadOnlyList<(string MapName, Difficulty Difficulty)> FindCoreMapDifficulties()
		{
			var allDifficulties = Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>();

			return FindCoreMaps()
				.SelectMany(map => allDifficulties.Select(difficulty => (MapName: map.Name, Difficulty: difficulty)))
				.ToList();
		}

		public bool TryFindMap(string mapName, out IMap map)
		{
			map = FindAllMaps().FirstOrDefault(x => x.Name.Equals(mapName, StringComparison.CurrentCultureIgnoreCase));
			return map != null;
		}

		private readonly Lazy<IConfigurationRoot> _configuration;

		private static readonly Lazy<IConfigurationRoot> LazyConfiguration
			= new Lazy<IConfigurationRoot>(() => new ConfigurationBuilder().SetBasePath(Program.ExecutableFolderPath).AddJsonFile("maps.json", optional: false, reloadOnChange: true).Build());
	}

	public class MapConfiguration
	{
		public List<Map> Maps { get; set; }
	}
}
