using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SloshyDoshMan.Service
{
	public class RequestLoggingMiddleware
	{
		public RequestLoggingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			try
			{
				await _next(context);
			}
			catch
			{
				Log.Information("Request from {RequestIP} for {RequestPath} ({RequestMethod}) failed!", context.Connection.RemoteIpAddress, RequestedPath(context), context.Request.Method);
				throw;
			}
			finally
			{
				Log.Information("Request from {RequestIP} for {RequestPath} {RequestMethod} took {RequestTimeInMilliseconds}ms", context.Connection.RemoteIpAddress, RequestedPath(context), context.Request.Method, stopWatch.ElapsedMilliseconds);
			}
		}

		private static string RequestedPath(HttpContext context)
		{
			var path = context.Request.Path;

			if (!string.IsNullOrEmpty(context.Request.QueryString.Value))
			{
				path += $"?{context.Request.QueryString.Value}";
			}

			return path;
		}

		private readonly RequestDelegate _next;
	}
}