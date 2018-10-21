using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SloshyDoshMan.Service.Maps
{
	[Route("webapi")]
	public class MapController : Controller
	{
		[HttpGet(nameof(Map))]
		public IActionResult Map([FromQuery] string mapName)
		{
			return Json(new MapStatisticsRequestHandler().HandleRequest(mapName));
		}
	}
}
