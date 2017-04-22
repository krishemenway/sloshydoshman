using System;

namespace SloshyDoshMan.HttpServer
{
	public class GracefulShutdownException : Exception
	{
		public GracefulShutdownException() : base("Waited for requests to finish, but requests didn't complete!") { }
	}
}