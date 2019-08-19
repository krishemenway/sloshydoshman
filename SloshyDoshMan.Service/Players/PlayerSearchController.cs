using Microsoft.AspNetCore.Mvc;
using SloshyDoshMan.Shared;
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

		[HttpGet(nameof(Search))]
		[ProducesResponseType(200, Type = typeof(Result<IReadOnlyList<PlayerProfile>>))]
		public ActionResult<Result<IReadOnlyList<PlayerProfile>>> Search([FromQuery] PlayerSearchRequest request)
		{
			var playerSearchResults = _playerStore
				.Search(request.Query)
				.Select(CreateProfile)
				.ToList();

			return Result<IReadOnlyList<PlayerProfile>>.Successful(playerSearchResults);
		}

		private PlayerProfile CreateProfile(Player player)
		{
			return new PlayerProfile
			{
				UserName = player.Name,
				SteamId = player.SteamId.ToString(),
				PerkStatistics = new List<PlayerPerkStatistic>(),
				MapStatistics = new List<PlayerMapStatistic>(),
			};
		}

		private readonly IPlayerStore _playerStore;
	}
}
