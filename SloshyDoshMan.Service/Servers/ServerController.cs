using System.Net.Http;
using System.Web.Http;

namespace SloshyDoshMan.Servers
{
	[RoutePrefix("api/servers")]
	public class ServerController : ApiController
	{
		public ServerController() : this(null) { }

		public ServerController(
			IServerStore serverStore = null,
			IRegisterServerRequestValidator registerRequestValidator = null)
		{
			_serverStore = serverStore ?? new ServerStore();
			_registerServerRequestValidator = registerRequestValidator ?? new RegisterServerRequestValidator();
		}
		
		[HttpPost]
		[Route("register")]
		public IHttpActionResult Register([FromBody] RegisterServerRequest registerServerRequest)
		{
			if(_registerServerRequestValidator.Validate(registerServerRequest))
			{
				var server = _serverStore.CreateNewServer(registerServerRequest.ServerName, Request.GetOwinContext().Request.RemoteIpAddress);
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
