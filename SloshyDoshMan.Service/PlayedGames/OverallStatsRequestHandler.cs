namespace SloshyDoshMan.Service.PlayedGames
{
	public class OverallStatsRequestHandler
	{
		public OverallStatsRequestHandler(IOverallStatsStore overallStatsStore = null)
		{
			_overallStatsStore = overallStatsStore ?? new OverallStatsStore();
		}

		public Result<OverallStats> HandleRequest()
		{
			var statistics = _overallStatsStore.CalculateStatistics();
			return Result<OverallStats>.Successful(statistics);
		}

		private readonly IOverallStatsStore _overallStatsStore;
	}
}
