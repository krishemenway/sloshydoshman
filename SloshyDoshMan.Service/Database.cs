using Microsoft.Extensions.Configuration;
using Npgsql;
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

		private static string DatabaseConnectionString => $"Host={Host};Username={User};Password={Password};Database={DatabaseName}";

		private static string Host => Program.Configuration.GetValue<string>("DatabaseHost");
		private static string DatabaseName => Program.Configuration.GetValue<string>("DatabaseName");

		private static string User => Program.Configuration.GetValue<string>("DatabaseUser");
		private static string Password => Program.Configuration.GetValue<string>("DatabasePassword");
	}
}