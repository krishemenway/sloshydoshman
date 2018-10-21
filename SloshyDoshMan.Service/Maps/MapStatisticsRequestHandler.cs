namespace SloshyDoshMan.Service.Maps
{
	public class MapStatisticsRequestHandler
	{
		public MapStatisticsRequestHandler(
			IMapStore mapStore = null,
			IMapStatisticsStore mapStatisticsStore = null)
		{
			_mapStore = mapStore ?? new MapStore();
			_mapStatisticsStore = mapStatisticsStore ?? new MapStatisticsStore();
		}

		public Result<MapStatisticsRepsonse> HandleRequest(string mapName)
		{
			if (!_mapStore.FindMap(mapName, out var map))
			{
				return Result<MapStatisticsRepsonse>.Failure("Unknown Map Received");
			}

			var response = new MapStatisticsRepsonse
				{
					PlayerMapScores = _mapStatisticsStore.FindTopPlayersForMap(map.Name)
				};

			return Result<MapStatisticsRepsonse>.Successful(response);
		}

		private readonly IMapStore _mapStore;
		private readonly IMapStatisticsStore _mapStatisticsStore;
	}
}
