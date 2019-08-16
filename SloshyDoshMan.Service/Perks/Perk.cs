using System;

namespace SloshyDoshMan.Service.Perks
{
	public class Perk
	{
		public string Name { get; set; }

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Perk, (o) => o.Name);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name);
		}
	}
}
