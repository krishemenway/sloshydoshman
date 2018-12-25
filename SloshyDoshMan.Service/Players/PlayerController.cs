using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SloshyDoshMan.Service.Players
{
	[Route("webapi/players")]
	public class PlayerController : Controller
	{
		[HttpGet("profile")]
		[ProducesResponseType(200, Type = typeof(Result<PlayerProfile>))]
		public IActionResult Profile([FromQuery] PlayerProfileRequest request)
		{
			return Json(new PlayerProfileRequestHandler().HandleRequest(request));
		}

		[HttpGet("search")]
		[ProducesResponseType(200, Type = typeof(Result<IReadOnlyList<PlayerProfile>>))]
		public IActionResult Search([FromQuery] PlayerSearchRequest request)
		{
			return Json(new PlayerSearchRequestHandler().HandleRequest(request));
		}
	}
}
