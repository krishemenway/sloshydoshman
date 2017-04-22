using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace SloshyDoshMan.Maps
{
	public class MapStore
	{
		public IReadOnlyList<Map> FindAllMaps()
		{
			const string sql = @"
				SELECT
					m.map as Name,
					m.is_workshop as IsWorkshop
				FROM server_map m";

			using (var connection = Database.CreateConnection())
			{
				return connection.Query<Map>(sql).ToList();
			}
		}
	}

	public class Map
	{
		public string Name { get; set; }
		public bool IsWorkshop { get; set; }
	}
}
