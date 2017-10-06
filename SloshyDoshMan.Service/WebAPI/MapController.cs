using KrisHemenway.Common;
using SloshyDoshMan.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SloshyDoshMan.WebAPI
{
	[RoutePrefix("webapi")]
	public class MapController : ApiController
	{
		[HttpGet]
		[Route("map")]
		public IHttpActionResult Map([FromUri] string mapName)
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

			return Ok(Result<MapStatsRepsonse>.Successful(mapStatsRepsonse));
		}
	}

	public class MapStatsRepsonse
	{
		public IReadOnlyList<PlayerMapScore> PlayerMapScores { get; set; }
	}
}
