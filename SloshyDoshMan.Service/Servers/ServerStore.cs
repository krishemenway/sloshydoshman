using Dapper;
using SloshyDoshMan.Shared;
using System;

namespace SloshyDoshMan.Service.Servers
{
	public interface IServerStore
	{
		IServer CreateNewServer(string serverName, string ipAddress);
	}

	internal class ServerStore : IServerStore
	{
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

				return new Server
					{
						ServerId = newServerId,
						LastKnownIPAddress = ipAddress,
						CurrentName = serverName
					};
			}
		}
	}
}
