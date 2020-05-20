using Microsoft.AspNetCore.Mvc;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.PlayedGames
{
	[ApiController]
	[Route("WebAPI/Games")]
	public class OverallStatsController : ControllerBase
	{
		public OverallStatsController(IOverallStatsStore overallStatsStore = null)
		{
			_overallStatsStore = overallStatsStore ?? new OverallStatsStore();
		}

		[HttpGet(nameof(Statistics))]
		[ProducesResponseType(200, Type = typeof(Result<OverallStats>))]
		public Result<OverallStats> Statistics()
		{
			var statistics = _overallStatsStore.CalculateStatistics();
			return Result<OverallStats>.Successful(statistics);
		}

		private readonly IOverallStatsStore _overallStatsStore;
	}
}
