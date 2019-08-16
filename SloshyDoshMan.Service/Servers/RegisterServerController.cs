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
			IRegisterServerRequestValidator registerRequestValidator = null)
		{
			_serverStore = serverStore ?? new ServerStore();
			_registerServerRequestValidator = registerRequestValidator ?? new RegisterServerRequestValidator();
		}

		[HttpPost(nameof(Register))]
		[ProducesResponseType(200, Type = typeof(Result<IServer>))]
		public ActionResult<Result<IServer>> Register([FromBody] RegisterServerRequest request)
		{
			if(!_registerServerRequestValidator.Validate(request, out var result))
			{
				return result.FailureOf<IServer>();
			}

			var remoteIPAddress = ControllerContext.HttpContext.Connection.RemoteIpAddress.ToString();
			var server = _serverStore.CreateNewServer(request.ServerName, remoteIPAddress);

			return Result<IServer>.Successful(server);
		}

		private readonly IServerStore _serverStore;
		private readonly IRegisterServerRequestValidator _registerServerRequestValidator;
	}
}
