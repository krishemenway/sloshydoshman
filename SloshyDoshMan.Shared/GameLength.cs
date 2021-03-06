﻿using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Shared
{
	public enum GameLength
	{
		Unknown,
		Short,
		Normal,
		Long,
	}

	public static class GameLengthHelpers
	{
		public static int FindTotalWavesForGameLength(GameLength gameLength)
		{
			return TotalWavesForGameLength[gameLength];
		}

		public static GameLength DetermineGameLengthFromLength(int totalWaves)
		{
			return TotalWavesForGameLength.SingleOrDefault(x => x.Value == totalWaves).Key;
		}

		private static readonly IReadOnlyDictionary<GameLength, int> TotalWavesForGameLength 
			= new Dictionary<GameLength, int>()
			{
				{ GameLength.Unknown, 0 },
				{ GameLength.Short, 4 },
				{ GameLength.Normal, 7 },
				{ GameLength.Long, 10 }
			};
	}
}
