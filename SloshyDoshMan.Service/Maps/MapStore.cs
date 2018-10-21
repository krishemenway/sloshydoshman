using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SloshyDoshMan.Service.Maps
{
	public class MapStore
	{
		public IReadOnlyList<Map> FindAllMaps()
		{
			return LazyMapList.Value;
		}

		private static IReadOnlyList<Map> LoadMapList()
		{
			using (var streamReader = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("SloshyDoshMan.Service.Maps.Maps.json")))
			{
				return JsonConvert.DeserializeObject<IReadOnlyList<Map>>(streamReader.ReadToEnd());
			}
		}

		public static Lazy<IReadOnlyList<Map>> LazyMapList = new Lazy<IReadOnlyList<Map>>(LoadMapList);
	}
}
