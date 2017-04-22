using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using Common.Logging;

namespace SloshyDoshMan.HttpServer
{
	public interface IGracefulOwinServer
	{
		IGracefulOwinServer WithHost(string host);
		IGracefulOwinServer WithPort(int port);
		IGracefulOwinServer WithShutdownTimeout(TimeSpan timeout);
		IGracefulOwinServer WithExceptionResponseGenerator(Action<IOwinContext> generator);
		IGracefulOwinServer Start(Action<IAppBuilder> buildAction);
		void Stop();
	}

	public class GracefulOwinServer : IGracefulOwinServer
	{
		public GracefulOwinServer()
		{
			_shutdownTimeout = TimeSpan.FromSeconds(8);
			_port = 80;
			_host = "+";
		}

		public IGracefulOwinServer WithHost(string host)
		{
			_host = host;
			return this;
		}

		public IGracefulOwinServer WithPort(int port)
		{
			_port = port;
			return this;
		}

		public IGracefulOwinServer WithShutdownTimeout(TimeSpan timeout)
		{
			_shutdownTimeout = timeout;
			return this;
		}

		public IGracefulOwinServer WithExceptionResponseGenerator(Action<IOwinContext> generator)
		{
			_exceptionResponseGenerator = generator;
			return this;
		}

		public IGracefulOwinServer Start(Action<IAppBuilder> buildAction)
		{
			if (_app != null)
			{
				Stop();
			}

			var options = new StartOptions(ServerUrl)
			{
				ServerFactory = "Microsoft.Owin.Host.HttpListener",
				Port = _port
			};

			_app = WebApp.Start(options, appBuilder => Startup(appBuilder, buildAction));
			Trace.Listeners.Remove("HostingTraceListener"); // OWIN directs trace output to console automatically upon start.  This stops that from happening...
			Log.Debug($"HTTP server now listening on {ServerUrl}");

			return this;
		}

		public void Stop()
		{
			Log.Debug("Waiting for in-flight requests to finish...");

			if (!GracefulShutdownMiddleware.Shutdown(_shutdownTimeout))
			{
				Log.Error(new GracefulShutdownException());
			}

			_app.Dispose();
			_app = null;
		}

		private void Startup(IAppBuilder appBuilder, Action<IAppBuilder> buildAction)
		{
			appBuilder.Use(UnhandledExceptionMiddlewareAsync);
			appBuilder.Use<GracefulShutdownMiddleware>();
			appBuilder.Use<RequestLoggingMiddleware>();
			appBuilder.Use(UnhandledExceptionResponseGeneratorMiddlewareAsync);

			buildAction.Invoke(appBuilder);
		}

		private static async Task UnhandledExceptionMiddlewareAsync(IOwinContext context, Func<Task> next)
		{
			try
			{
				await next.Invoke();
			}
			catch (Exception x)
			{
				if (!context.Request.CallCancelled.IsCancellationRequested)
				{
					Log.Error(x);
				}
			}
		}

		private async Task UnhandledExceptionResponseGeneratorMiddlewareAsync(IOwinContext context, Func<Task> next)
		{
			try
			{
				await next.Invoke();
			}
			catch (Exception)
			{
				if (!context.Request.CallCancelled.IsCancellationRequested)
				{
					ExecuteExceptionResponseGenerator(context);
				}

				throw;
			}
		}

		private void ExecuteExceptionResponseGenerator(IOwinContext context)
		{
			if (_exceptionResponseGenerator != null)
			{
				try
				{
					_exceptionResponseGenerator(context);
				}
				catch (Exception exception)
				{
					Log.Error(exception);
				}
			}
		}

		private string ServerUrl
		{
			get { return $"http://{_host}:{_port}"; }
		}

		private IDisposable _app;

		internal static ILog Log
		{
			get { return LazyLog.Value; }
		}

		private static readonly Lazy<ILog> LazyLog = new Lazy<ILog>(() => LogManager.GetLogger<GracefulOwinServer>());

		private Action<IOwinContext> _exceptionResponseGenerator;
		private int _port;
		private string _host;
		private TimeSpan _shutdownTimeout;
	}
}