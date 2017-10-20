using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data.Common;

namespace SloshyDoshMan.Service
{
	public static class Database
	{
		public static DbConnection CreateConnection()
		{
			var connection = new NpgsqlConnection($"Host={Host};Username={User};Password={Password};Database={DatabaseName}");
			connection.Open();
			return connection;
		}

		public static string DatabaseName
		{
			get { return Program.Configuration.GetValue<string>("DatabaseName"); }
		}

		public static string Host
		{
			get { return Program.Configuration.GetValue<string>("DatabaseHost"); }
		}

		public static string User
		{
			get { return Program.Configuration.GetValue<string>("DatabaseUser"); }
		}

		public static string Password
		{
			get { return Program.Configuration.GetValue<string>("DatabasePassword"); }
		}
	}
}