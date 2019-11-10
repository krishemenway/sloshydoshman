using System;
using Microsoft.AspNetCore.Mvc;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.Servers
{
	[ApiController]
	[Route("api/servers")]
	public class RegisterServerController : ControllerBase
	{
		public RegisterServerController(
			IServerStore serverStore = null,
			IRegisterServerRequestValidator registerRequestValidator = null,
			Func<string> findRemoteIPAddressFunc = null)
		{
			_serverStore = serverStore ?? new ServerStore();
			_registerServerRequestValidator = registerRequestValidator ?? new RegisterServerRequestValidator();
			_findRemoteIPAddressFunc = findRemoteIPAddressFunc ?? (() => ControllerContext.HttpContext.Connection.RemoteIpAddress.ToString());
		}

		[HttpPost(nameof(Register))]
		[ProducesResponseType(200, Type = typeof(Result<IServer>))]
		public ActionResult<Result<IServer>> Register([FromBody] RegisterServerRequest request)
		{
			if(!_registerServerRequestValidator.Validate(request, out var result))
			{
				return result.FailureOf<IServer>();
			}

			var server = _serverStore.CreateNewServer(request.ServerName, _findRemoteIPAddressFunc());
			return Result<IServer>.Successful(server);
		}

		private readonly IServerStore _serverStore;
		private readonly IRegisterServerRequestValidator _registerServerRequestValidator;
		private readonly Func<string> _findRemoteIPAddressFunc;
	}
}
