namespace SloshyDoshMan.Shared
{
	public class PlayerGameState
	{
		public long SteamId { get; set; }
		public string Name { get; set; }
		public string IPAddress { get; set; }
		public string UniqueNetId { get; set; }

		public string Perk { get; set; }
		public int Dosh { get; set; }
		public int Health { get; set; }
		public int Kills { get; set; }
		public int Ping { get; set; }
	}
}
