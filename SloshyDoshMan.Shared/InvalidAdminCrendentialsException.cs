using System;

namespace SloshyDoshMan.Shared
{
	public class InvalidAdminCrendentialsException : Exception
	{
		public InvalidAdminCrendentialsException() 
			: base("Invalid admin credentials provided. Make sure your credentials match and that bHttpAuth=true in KFWebAdmin.ini") { }
	}
}
