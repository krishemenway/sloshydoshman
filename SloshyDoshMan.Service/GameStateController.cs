using System.Web.Http;
using SloshyDoshMan.Shared;
using SloshyDoshMan.Players;
using SloshyDoshMan.PlayedGames;

namespace SloshyDoshMan
{
	[RoutePrefix("api/kf2/gamestate")]
	public class GameStateController : ApiController
	{
		[HttpPost]
		[Route("save")]
		public IHttpActionResult SaveGameState([FromBody] GameState newGameState)
		{
			new RefreshGameStateAction().RefreshGameState(newGameState);
			return Ok();
		}
	}
}
