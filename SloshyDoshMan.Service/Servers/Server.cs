using System;

namespace SloshyDoshMan.Service.Servers
{
	public interface IServer
	{
		Guid ServerId { get; }
		string CurrentName { get; }
		string LastKnownIPAddress { get; }
	}

	public class Server : IServer
	{
		public Guid ServerId { get; set; }
		public string CurrentName { get; set; }
		public string LastKnownIPAddress { get; set; }
	}
}
