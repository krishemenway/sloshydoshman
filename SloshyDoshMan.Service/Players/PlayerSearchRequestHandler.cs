using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Service.Players
{
	public class PlayerSearchRequestHandler
	{
		public PlayerSearchRequestHandler(IPlayerStore playerStore = null)
		{
			_playerStore = playerStore ?? new PlayerStore();
		}

		public Result<IReadOnlyList<PlayerProfile>> HandleRequest(PlayerSearchRequest request)
		{
			var playerSearchResults = _playerStore.Search(request.Query)
				.Select(x => new PlayerProfile { UserName = x.Name, SteamId = x.SteamId.ToString(), PerkStatistics = new List<PlayerPerkStatistic>(), MapStatistics = new List<PlayerMapStatistic>() })
				.ToList();

			return Result<IReadOnlyList<PlayerProfile>>.Successful(playerSearchResults);
		}

		private readonly IPlayerStore _playerStore;
	}
}
