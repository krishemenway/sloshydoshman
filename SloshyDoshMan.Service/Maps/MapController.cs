using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.Maps
{
	[Route("webapi")]
	public class MapController : Controller
	{
		[HttpGet(nameof(Map))]
		public IActionResult Map([FromQuery] string mapName)
		{
			var serverMaps = new MapStore().FindAllMaps();
			
			if(!serverMaps.Any(x => x.Name.Equals(mapName, StringComparison.CurrentCultureIgnoreCase)))
			{
				return Ok(Result.Failure("Unknown Map Received"));
			}

			var mapStatsRepsonse = new MapStatsRepsonse
			{
				PlayerMapScores = new MapStatisticsStore().FindTopPlayersForMap(mapName)
			};

			return Json(Result<MapStatsRepsonse>.Successful(mapStatsRepsonse));
		}
	}

	public class MapStatsRepsonse
	{
		public IReadOnlyList<PlayerMapScore> PlayerMapScores { get; set; }
	}
}
