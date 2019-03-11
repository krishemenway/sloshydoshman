using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.Players
{
	[Route("webapi/players")]
	public class PlayerSearchController : Controller
	{
		public PlayerSearchController(IPlayerStore playerStore = null)
		{
			_playerStore = playerStore ?? new PlayerStore();
		}

		[HttpGet("search")]
		[ProducesResponseType(200, Type = typeof(Result<IReadOnlyList<PlayerProfile>>))]
		public IActionResult HandleRequest([FromQuery] PlayerSearchRequest request)
		{
			var playerSearchResults = _playerStore.Search(request.Query)
				.Select(x => new PlayerProfile { UserName = x.Name, SteamId = x.SteamId.ToString(), PerkStatistics = new List<PlayerPerkStatistic>(), MapStatistics = new List<PlayerMapStatistic>() })
				.ToList();

			return Json(Result<IReadOnlyList<PlayerProfile>>.Successful(playerSearchResults));
		}

		private readonly IPlayerStore _playerStore;
	}
}
