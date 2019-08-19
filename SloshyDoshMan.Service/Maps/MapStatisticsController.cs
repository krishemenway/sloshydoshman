using Microsoft.AspNetCore.Mvc;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.Maps
{
	[ApiController]
	[Route("webapi/maps")]
	public class MapStatisticsController : ControllerBase
	{
		public MapStatisticsController(
			IMapStore mapStore = null,
			IMapStatisticsStore mapStatisticsStore = null)
		{
			_mapStore = mapStore ?? new MapStore();
			_mapStatisticsStore = mapStatisticsStore ?? new MapStatisticsStore();
		}

		[HttpGet(nameof(MapStatistics))]
		[ProducesResponseType(200, Type = typeof(Result<MapStatisticsRepsonse>))]
		public ActionResult<Result<MapStatisticsRepsonse>> MapStatistics([FromQuery] MapStatisticsRequest request)
		{
			if (!_mapStore.TryFindMap(request.MapName, out var map))
			{
				return Result<MapStatisticsRepsonse>.Failure($"Unknown Map: {request.MapName}");
			}

			var response = new MapStatisticsRepsonse
				{
					PlayerMapScores = _mapStatisticsStore.FindTopPlayersForMap(map.Name),
				};

			return Result<MapStatisticsRepsonse>.Successful(response);
		}

		private readonly IMapStore _mapStore;
		private readonly IMapStatisticsStore _mapStatisticsStore;
	}
}
