using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace SloshyDoshMan.HttpServer
{
	public class WebApiExceptionHandler : IExceptionHandler
	{
		public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
		{
			var info = ExceptionDispatchInfo.Capture(context.Exception);
			info.Throw();
			return Task.FromResult(0);
		}
	}
}