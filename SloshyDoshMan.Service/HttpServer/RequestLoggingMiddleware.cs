using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace SloshyDoshMan.HttpServer
{
	public class RequestLoggingMiddleware : OwinMiddleware
	{
		public RequestLoggingMiddleware(OwinMiddleware next)
			: base(next)
		{
		}

		public override async Task Invoke(IOwinContext context)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			// Capture these in case the client disconnects on us and OWIN disposes them
			var method = context.Request.Method;
			var uri = context.Request.Uri;

			try
			{
				await Next.Invoke(context);
			}
			finally
			{
				stopwatch.Stop();
				
				var logMessage = context.Request.CallCancelled.IsCancellationRequested
					? $"{method} {uri} - CLIENT DISCONNECTED - {stopwatch.ElapsedMilliseconds} ms"
					: $"{method} {uri} - {context.Response.StatusCode} - {stopwatch.ElapsedMilliseconds} ms";

				GracefulOwinServer.Log.Debug(logMessage);
			}
		}
	}
}