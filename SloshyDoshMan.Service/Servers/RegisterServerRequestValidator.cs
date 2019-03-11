namespace SloshyDoshMan.Service.Servers
{
	public interface IRegisterServerRequestValidator
	{
		bool Validate(RegisterServerRequest request, out Result result);
	}

	class RegisterServerRequestValidator : IRegisterServerRequestValidator
	{
		public bool Validate(RegisterServerRequest request, out Result result)
		{
			result = null;
			return true;
		}
	}
}
