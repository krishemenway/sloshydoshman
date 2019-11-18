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

		private static string Host => Program.Configuration.GetValue<string>("SloshyDoshManDatabaseHost");
		private static string DatabaseName => Program.Configuration.GetValue<string>("SloshyDoshManDatabaseName");

		private static string User => Program.Configuration.GetValue<string>("SloshyDoshManDatabaseUser");
		private static string Password => Program.Configuration.GetValue<string>("SloshyDoshManDatabasePassword");
	}
}