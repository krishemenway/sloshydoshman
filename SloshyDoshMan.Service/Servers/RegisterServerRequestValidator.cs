using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SloshyDoshMan.Servers
{
	public interface IRegisterServerRequestValidator
	{
		bool Validate(RegisterServerRequest request);
	}

	class RegisterServerRequestValidator : IRegisterServerRequestValidator
	{
		public bool Validate(RegisterServerRequest request)
		{
			return true;
		}
	}
}
