﻿using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.Servers
{
	public interface IRegisterServerRequestValidator
	{
		bool Validate(RegisterServerRequest request, out Result result);
	}

	public class RegisterServerRequestValidator : IRegisterServerRequestValidator
	{
		public RegisterServerRequestValidator(ISettings settings = null)
		{
			_settings = settings ?? Program.Settings;
		}

		public bool Validate(RegisterServerRequest request, out Result result)
		{
			if (!string.IsNullOrEmpty(request.ServerName) && request.ServerName.Length > 1024)
			{
				result = Result.Failure($"Server name must be between 1 and {MaxServerNameLength}");
				return false;
			}

			if (_settings.ServerRegistrationKey != request.RegistrationKey)
			{
				result = Result.Failure("Invalid Registration Key");
				return false;
			}

			result = Result.Successful;
			return true;
		}

		public const int MaxServerNameLength = 1024;
		private readonly ISettings _settings;
	}
}
