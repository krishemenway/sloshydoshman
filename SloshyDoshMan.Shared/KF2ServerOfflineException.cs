using System;

namespace SloshyDoshMan.Shared
{
	public class KF2ServerOfflineException : Exception
	{
		public KF2ServerOfflineException() 
			: base("Unable to contact KF2 Server. Server most likely offline or web admin is not enabled.") { }
	}
}
