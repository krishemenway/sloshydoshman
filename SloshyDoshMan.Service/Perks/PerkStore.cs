using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SloshyDoshMan.Service.Perks
{
	public interface IPerkStore
	{
		IReadOnlyList<Perk> FindAllPerks();
	}

	public class PerkStore : IPerkStore
	{
		public PerkStore(Lazy<IConfigurationRoot> configuration = null)
		{
			_configuration = configuration ?? LazyConfiguration;
		}

		public IReadOnlyList<Perk> FindAllPerks()
		{
			return _configuration.Value.Get<PerksConfiguration>().Perks;
		}

		private readonly Lazy<IConfigurationRoot> _configuration;

		private static readonly Lazy<IConfigurationRoot> LazyConfiguration
			= new Lazy<IConfigurationRoot>(() => new ConfigurationBuilder().SetBasePath(Program.ExecutableFolderPath).AddJsonFile("perks.json", optional: false, reloadOnChange: true).Build());
	}

	public class PerksConfiguration
	{
		public List<Perk> Perks { get; set; }
	}
}
