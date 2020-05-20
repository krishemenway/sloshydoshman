using Npgsql;
using System.Collections.Generic;
using System.Data.Common;

namespace SloshyDoshMan.Service
{
	public static class Database
	{
		public static DbConnection CreateConnection()
		{
			var connection = new NpgsqlConnection(DatabaseConnectionString);
			connection.Open();
			return connection;
		}

		private static readonly IReadOnlyList<string> ConnectionStringParts = new[]
		{
			$"Host={Program.Settings.SloshyDoshManDatabaseHost};",
			$"Username={Program.Settings.SloshyDoshManDatabaseUser};",
			$"Password={Program.Settings.SloshyDoshManDatabasePassword};",
			$"Database={Program.Settings.SloshyDoshManDatabaseName};",
			$"Port={Program.Settings.SloshyDoshManDatabasePort}"
		};

		private static string DatabaseConnectionString = string.Join("", ConnectionStringParts);
	}
}