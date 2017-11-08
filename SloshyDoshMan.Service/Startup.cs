using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SloshyDoshMan.Service
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvcCore().AddJsonFormatters(FixJsonCamelCasing);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseMvc();
		}

		private void FixJsonCamelCasing(JsonSerializerSettings settings)
		{
			if (settings.ContractResolver is DefaultContractResolver resolver)
			{
				resolver.NamingStrategy = null;  // <<!-- this removes the camelcasing
			}
		}

		public IConfiguration Configuration { get; }
	}
}
