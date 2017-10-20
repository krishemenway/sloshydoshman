using Microsoft.AspNetCore.Mvc;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.PlayedGameState
{
	[Route("api/kf2/gamestate")]
	public class GameStateController : Controller
	{
		[HttpPost("save")]
		public IActionResult SaveGameState([FromBody] GameState newGameState)
		{
			new RefreshGameStateAction().RefreshGameState(newGameState);
			return Ok();
		}
	}
}
