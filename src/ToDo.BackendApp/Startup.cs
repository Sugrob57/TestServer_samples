using Microsoft.AspNetCore.Cors.Infrastructure;
using ToDo.BackendApp.Models;
using ToDo.BackendApp.Services.StorageContexts;
using ToDo.BackendApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ToDo.BackendApp
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<ApplicationSettings>();
			services.AddSingleton<IStorageContext<Todo>, DatabaseTodoContext>();
			services.AddTransient<DataStorage>();
			services.AddTransient<IEmailService, RemoteSmtpEmailService>();
			services.AddTransient<ITodoService, TodoService>();

			services
				.AddCors(options =>
				{
					CorsPolicyBuilder corsPolicyBuilder = new CorsPolicyBuilder(new string[] { });
					corsPolicyBuilder
							.SetIsOriginAllowed(origin => true)
							.AllowCredentials()
							.AllowAnyMethod()
							.AllowAnyHeader();

					CorsPolicy corsPolicy = corsPolicyBuilder.Build();
					corsPolicy.IsOriginAllowed = origin => true;

					options.AddPolicy(
						name: "defaultCorsPolicy",
						corsPolicy);
				});

			services
				.AddMvc()
				.AddApplicationPart(typeof(Program).Assembly);

			services
				.AddControllers()
				.AddControllersAsServices();

			services.AddSwaggerGen();

			
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseRouting();
			app.UseCors("defaultCorsPolicy");
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseHttpLogging();

			app.UseDeveloperExceptionPage();

			app.UseSwagger();
			app.UseSwaggerUI();
			app.UseHttpsRedirection();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}