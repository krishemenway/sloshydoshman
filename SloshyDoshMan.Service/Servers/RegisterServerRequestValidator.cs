namespace SloshyDoshMan.Service.Servers
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
