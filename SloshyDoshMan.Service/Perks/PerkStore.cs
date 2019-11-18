using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SloshyDoshMan.Service.Perks
{
	public interface IPerkStore
	{
		IReadOnlyList<Perk> FindAllPerks();
	}

	public class PerkStore : IPerkStore
	{
		public IReadOnlyList<Perk> FindAllPerks()
		{
			return LazyPerkList.Value.ToList();
		}

		private static IReadOnlyList<Perk> LoadPerkList()
		{
			using (var streamReader = new StreamReader(PerksResourceStream))
			{
				return JsonConvert.DeserializeObject<IReadOnlyList<Perk>>(streamReader.ReadToEnd());
			}
		}

		private static Stream PerksResourceStream => File.Open(Path.Combine(Program.ExecutablePath, "perks.json"), FileMode.Open);
		private static readonly Lazy<IReadOnlyList<Perk>> LazyPerkList = new Lazy<IReadOnlyList<Perk>>(LoadPerkList);
	}
}
