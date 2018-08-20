using System;
using System.Threading.Tasks;
using GrainInterfaces.GrainInterfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Swashbuckle.AspNetCore.Swagger;

namespace ManagementApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			services.AddSingleton(CreateClusterClient);
			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info {Title = "Timebattle Management API", Version = "v1"});
				//c.OperationFilter<RequiredScopeOperationFilter>();
			});
		}

		private IClusterClient CreateClusterClient(IServiceProvider serviceProvider)
		{
			var log              = serviceProvider.GetService<ILogger<Startup>>();
			var connectionString = Environment.GetEnvironmentVariable("CLUSTER_CONNECTION_STRING") ?? "mongodb://localhost/Timebattle";

			var client = new ClientBuilder()
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = "timebattle.server";
					options.ServiceId = "TimebattleManagementService";
				}).UseMongoDBClustering(_ =>
				{
					_.ConnectionString = connectionString;
					_.DatabaseName = "Timebattle";
				})
				.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IPlayerGrain).Assembly))
				.Build();

			client.Connect(RetryFilter).GetAwaiter().GetResult();
			return client;

			async Task<Boolean> RetryFilter(Exception exception)
			{
				log?.LogWarning("Exception while attempting to connect to Orleans cluster: {Exception}", exception);
				await Task.Delay(TimeSpan.FromSeconds(2));
				return true;
			}
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			else
				app.UseHsts();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Timebattle Management API V1"); });

			app.UseHttpsRedirection();
			app.UseMvc();
		}
	}
}