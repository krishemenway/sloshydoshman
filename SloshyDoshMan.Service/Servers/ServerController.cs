using Microsoft.AspNetCore.Mvc;

namespace SloshyDoshMan.Service.Servers
{
	[Route("api/servers")]
	public class ServerController : Controller
	{
		public ServerController() : this(null) { }

		public ServerController(
			IServerStore serverStore = null,
			IRegisterServerRequestValidator registerRequestValidator = null)
		{
			_serverStore = serverStore ?? new ServerStore();
			_registerServerRequestValidator = registerRequestValidator ?? new RegisterServerRequestValidator();
		}

		[HttpPost("register")]
		public IActionResult Register([FromBody] RegisterServerRequest registerServerRequest)
		{
			if(_registerServerRequestValidator.Validate(registerServerRequest))
			{
				var server = _serverStore.CreateNewServer(registerServerRequest.ServerName, Request.HttpContext.Connection.RemoteIpAddress.ToString());
				return Ok(server);
			}
			
			return BadRequest("Bitch");
		}

		private readonly IServerStore _serverStore;
		private readonly IRegisterServerRequestValidator _registerServerRequestValidator;
	}

	public class RegisterServerRequest
	{
		public string KF2ServerIP { get; set; }
		public string ServerName { get; set; }
	}
}
