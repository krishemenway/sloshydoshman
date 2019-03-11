using Microsoft.AspNetCore.Mvc;
using SloshyDoshMan.Shared;

namespace SloshyDoshMan.Service.Servers
{
	[Route("api/servers")]
	public class RegisterServerController : Controller
	{
		public RegisterServerController(
			IServerStore serverStore = null,
			IRegisterServerRequestValidator registerRequestValidator = null)
		{
			_serverStore = serverStore ?? new ServerStore();
			_registerServerRequestValidator = registerRequestValidator ?? new RegisterServerRequestValidator();
		}

		[HttpPost("register")]
		[ProducesResponseType(200, Type = typeof(Result<IServer>))]
		public IActionResult HandleRequest([FromBody] RegisterServerRequest request)
		{
			if(!_registerServerRequestValidator.Validate(request, out var result))
			{
				return Ok(result);
			}

			var server = _serverStore.CreateNewServer(request.ServerName, Request.HttpContext.Connection.RemoteIpAddress.ToString());
			return Ok(Result<IServer>.Successful(server));
		}

		private readonly IServerStore _serverStore;
		private readonly IRegisterServerRequestValidator _registerServerRequestValidator;
	}
}
