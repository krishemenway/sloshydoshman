using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

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
			services.AddMvcCore().AddJsonOptions(FixJsonCamelCasing);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			app.UseMiddleware<RequestLoggingMiddleware>();
			app.UseRouting();
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}

		private void FixJsonCamelCasing(JsonOptions options)
		{
			// this unsets the default behavior (camelCase); "what you see is what you get" is now default
			options.JsonSerializerOptions.PropertyNamingPolicy = null;
			options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		}

		public IConfiguration Configuration { get; }
	}
}
