﻿using System;
using System.Collections.Generic;

namespace SloshyDoshMan.Shared
{
	public interface IGameState
	{
		Guid ServerId { get; }
		string ServerName { get; }

		int CurrentWave { get; set; }
		int TotalWaves { get; }

		string Map { get; }
		string Difficulty { get; }
		string GameType { get; }

		List<PlayerGameState> Players { get; }
	}

	public class GameState : IGameState
	{
		public Guid ServerId { get; set; }
		public string ServerName { get; set; }

		public int CurrentWave { get; set; }
		public int TotalWaves { get; set; }

		public string Map { get; set; }
		public string Difficulty { get; set; }
		public string GameType { get; set; }

		public List<PlayerGameState> Players { get; set; }

		public GameLength GameLength
		{
			get { return GameLengthHelpers.DetermineGameLengthFromLength(TotalWaves); }
		}
	}
}
