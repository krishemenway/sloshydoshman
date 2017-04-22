using System;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Owin;

namespace SloshyDoshMan.HttpServer
{
	public static class GracefulOwinServerExtensions
	{
		public static IGracefulOwinServer StartWithWebApi(this IGracefulOwinServer server, Action<HttpConfiguration> customWebApiConfiguration = null, Action<IAppBuilder> buildAction = null)
		{
			return server.Start(appBuilder => ConfigureApp(appBuilder, customWebApiConfiguration, buildAction));
		}

		private static void ConfigureApp(IAppBuilder app, Action<HttpConfiguration> customWebApiConfiguration, Action<IAppBuilder> buildAction)
		{
			if (buildAction != null)
			{
				buildAction(app);
			}

			var config = new HttpConfiguration();
			config.MapHttpAttributeRoutes();
			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Never;
			config.Services.Replace(typeof(IExceptionHandler), new WebApiExceptionHandler());
			config.Formatters.Remove(config.Formatters.XmlFormatter);

			if (customWebApiConfiguration != null)
			{
				customWebApiConfiguration(config);
			}

			app.UseWebApi(config);
		}
	}
}