using Microsoft.AspNetCore.Mvc;

namespace SloshyDoshMan.Service.PlayedGames
{
	[Route("webapi/games")]
	public class PlayedGameController : Controller
	{
		[HttpGet(nameof(Profile))]
		[ProducesResponseType(200, Type = typeof(Result<PlayedGameProfile>))]
		public IActionResult Profile([FromQuery] PlayedGameProfileRequest request)
		{
			return Json(new PlayedGameProfileRequestHandler().HandleRequest(request));
		}

		[HttpGet(nameof(Recent))]
		[ProducesResponseType(200, Type = typeof(Result<PlayedGameProfile>))]
		public IActionResult Recent([FromQuery] RecentGamesRequest request)
		{
			return Json(new RecentGamesRequestHandler().HandleRequest(request));
		}

		[HttpGet(nameof(Statistics))]
		[ProducesResponseType(200, Type = typeof(Result<OverallStats>))]
		public IActionResult Statistics()
		{
			return Json(new OverallStatsRequestHandler().HandleRequest());
		}
	}
}
