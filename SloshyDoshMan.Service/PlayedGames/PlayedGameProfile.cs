namespace SloshyDoshMan.Service.PlayedGames
{
	public class PlayedGameProfile
	{
		public IPlayedGame PlayedGame { get; set; }
		public IScoreboard Scoreboard { get; set; }
	}
}
