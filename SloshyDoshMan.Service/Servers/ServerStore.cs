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
				(ServerId, ServerName, LastKnownIPAddress)
				VALUES
				(@ServerId, @ServerName, @IpAddress)";

			using(var connection = Database.CreateConnection())
			{
				var newServerId = Guid.NewGuid();

				connection.Execute(sql, new { ServerId = newServerId, serverName, ipAddress });

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
