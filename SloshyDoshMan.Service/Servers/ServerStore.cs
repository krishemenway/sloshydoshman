using Dapper;
using SloshyDoshMan.Shared;
using System;
using System.Linq;

namespace SloshyDoshMan.Service.Servers
{
	public interface IServerStore
	{
		bool TryFindServer(Guid serverId, out IServer server);
		IServer CreateNewServer(string serverName, string ipAddress);
	}

	public class ServerStore : IServerStore
	{
		public bool TryFindServer(Guid serverId, out IServer server)
		{
			const string sql = @"
				SELECT
					server_id as serverid,
					name,
					last_known_ip as lastknownip
				FROM server
				WHERE server_id = @ServerId";

			using (var connection = Database.CreateConnection())
			{
				server = connection
					.Query<ServerRecord>(sql, new { serverId })
					.Select((serverRecord) => CreateServer(serverRecord.ServerId, serverRecord.Name, serverRecord.LastKnownIP))
					.SingleOrDefault();

				return server != null;
			}
		}

		public IServer CreateNewServer(string serverName, string ipAddress)
		{
			const string sql = @"
				INSERT INTO server
				(server_id, name, last_known_ip)
				VALUES
				(uuid_generate_v4(), @ServerName, @IpAddress)
				RETURNING server_id";

			using(var connection = Database.CreateConnection())
			{
				var newServerId = connection.QuerySingle<Guid>(sql, new { serverName, ipAddress });
				return CreateServer(newServerId, serverName, ipAddress);
			}
		}

		private IServer CreateServer(Guid serverId, string name, string ipAddress)
		{
			return new Server
			{
				ServerId = serverId,
				CurrentName = name,
				LastKnownIPAddress = ipAddress,
			};
		}

		private class ServerRecord
		{
			public Guid ServerId { get; set; }
			public string Name { get; set; }
			public string LastKnownIP { get; set; }
		}
	}
}
