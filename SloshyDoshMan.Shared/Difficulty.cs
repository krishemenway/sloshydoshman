using System;
using System.Text.RegularExpressions;

namespace SloshyDoshMan.Shared
{
	public enum Difficulty
	{
		Hard,
		Suicidal,
		HellOnEarth
	}

	public static class DifficultyHelpers
	{
		public static string Convert(Difficulty difficulty)
		{
			switch(difficulty)
			{
				case Difficulty.HellOnEarth:
					return "Hell on Earth";
				default:
					return difficulty.ToString();
			}
		}

		public static Difficulty Convert(string difficulty)
		{
			return (Difficulty)Enum.Parse(typeof(Difficulty), Regex.Replace(difficulty, @"(^\w)|(\s\w)", m => m.Value.ToUpper()).Replace(" ", ""));
		}
	}
}
