using SloshyDoshMan.Shared;

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

		public Result<MapStatisticsRepsonse> HandleRequest(MapStatisticsRequest request)
		{
			if (!_mapStore.FindMap(request.MapName, out var map))
			{
				return Result<MapStatisticsRepsonse>.Failure($"Unknown Map: {request.MapName}");
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
