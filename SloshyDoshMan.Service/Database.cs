using Npgsql;
using System;
using System.Data.Common;

namespace SloshyDoshMan
{
	public static class Database
	{
		public static DbConnection CreateConnection()
		{
			var connection = new NpgsqlConnection($"Host={Host};Username={User};Password={Password};Database={DatabaseName}");
			connection.Open();
			return connection;
		}

		public static string Host
		{
			get { return Environment.GetEnvironmentVariable("PushServiceHost", EnvironmentVariableTarget.Machine); }
		}

		public static string User
		{
			get { return Environment.GetEnvironmentVariable("PushServiceUser", EnvironmentVariableTarget.Machine); }
		}

		public static string Password
		{
			get { return Environment.GetEnvironmentVariable("PushServicePassword", EnvironmentVariableTarget.Machine); }
		}

		public const string DatabaseName = "kf2stats";
	}
}