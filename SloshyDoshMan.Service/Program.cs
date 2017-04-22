using Topshelf;
using Topshelf.Common.Logging;

namespace SloshyDoshMan
{
	class Program
	{
		public static void Main()
		{
			HostFactory.Run(x =>
			{
				x.Service<Service>(s =>
				{
					s.ConstructUsing(name => new Service());
					s.WhenStarted(tc => tc.Start());
					s.WhenStopped(tc => tc.Stop());
				});

				x.RunAsLocalSystem();
				x.StartAutomaticallyDelayed();
				x.UseCommonLogging();

				x.SetDescription("SloshyDoshMan api and management service");
				x.SetDisplayName("SloshyDoshManService");
				x.SetServiceName("SloshyDoshManService");
			});
		}

	}
}
