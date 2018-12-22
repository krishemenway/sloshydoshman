using Microsoft.AspNetCore.Mvc;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.PlayedGameState
{
	[Route("api/kf2/gamestate")]
	public class SaveGameStateController : Controller
	{
		[HttpPost("save")]
		public IActionResult SaveGameState([FromBody] GameState gameStateRequest)
		{
			return Ok(new SaveGameStateRequestHandler().HandleRequest(gameStateRequest));
		}
	}
}
