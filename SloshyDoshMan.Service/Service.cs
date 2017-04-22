using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.NLog;
using Microsoft.Owin;
using Newtonsoft.Json;
using SloshyDoshMan.HttpServer;
using System.IO;
using System.Net;
using System.Reflection;

namespace SloshyDoshMan
{
	public class Service
	{
		static Service()
		{
			LogManager.Adapter = new NLogLoggerFactoryAdapter(new NameValueCollection
			{
				{ "configType", "FILE" },
				{ "configFile", Path.Combine(ExecutingDirectory, ConfigFilePath) }
			});
		}

		public void Start()
		{
			_webApp = new GracefulOwinServer()
				.WithPort(8098)
				.WithExceptionResponseGenerator(CustomExceptionResponseGenerator)
				.StartWithWebApi();
		}

		public void Stop()
		{
			_webApp.Stop();
			_webApp = null;
		}

		private static void CustomExceptionResponseGenerator(IOwinContext context)
		{
			var response = new { Success = false };

			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			context.Response.ContentType = "application/json";
			context.Response.Write(JsonConvert.SerializeObject(response));
		}

		private static string ExecutingDirectory
		{
			get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
		}

		private static string ConfigFilePath
		{
			get
			{
#if DEBUG
				return "log.debug.config";
#else
				return "log.release.config";
#endif
			}
		}

		private static IGracefulOwinServer _webApp;
	}
}
