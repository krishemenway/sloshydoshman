using Microsoft.AspNetCore.Mvc;

namespace SloshyDoshMan.Service.Maps
{
	[Route("webapi")]
	public class MapController : Controller
	{
		[HttpGet(nameof(Map))]
		public IActionResult Map([FromQuery] MapStatisticsRequest request)
		{
			return Json(new MapStatisticsRequestHandler().HandleRequest(request));
		}
	}
}
