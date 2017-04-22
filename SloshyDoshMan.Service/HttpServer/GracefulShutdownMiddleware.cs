using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace SloshyDoshMan.HttpServer
{
	public class GracefulShutdownMiddleware : OwinMiddleware
	{
		public GracefulShutdownMiddleware(OwinMiddleware next)
			: base(next) { }

		public static bool Shutdown(TimeSpan timeout)
		{
			_shuttingDown = true;

			var sleepResult = SpinWait.SpinUntil(() => _activeRequestCount <= 0, timeout);
			Thread.Sleep(500); // Give OWIN time to finish sending responses for requests that just finished
			return sleepResult;
		}

		public override async Task Invoke(IOwinContext context)
		{
			Interlocked.Increment(ref _activeRequestCount);

			try
			{
				if (_shuttingDown)
				{
					context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
					return;
				}

				await Next.Invoke(context);
			}
			finally
			{
				Interlocked.Decrement(ref _activeRequestCount);
			}
		}

		private static bool _shuttingDown;
		private static int _activeRequestCount;
	}
}