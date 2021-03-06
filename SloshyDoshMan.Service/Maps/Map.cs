﻿using System;

namespace SloshyDoshMan.Service.Maps
{
	public interface IMap
	{
		string Name { get; }
		bool IsWorkshop { get; }
	}

	public class Map : IMap
	{
		public string Name { get; set; }
		public bool IsWorkshop { get; set; }

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Map, (o) => o.Name);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name);
		}
	}
}
