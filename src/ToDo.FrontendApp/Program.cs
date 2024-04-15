using Microsoft.AspNetCore.Cors.Infrastructure;

namespace ToDo.FrontendApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder
				.Configuration
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("commonAppSettings.json");

			builder.Services.AddControllersWithViews();

			builder
				.Services
				.AddCors(
					options =>
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


			var app = builder.Build();

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseDefaultFiles();
			app.UseRouting();
			app.UseAuthorization();
			app.UseCors("defaultCorsPolicy");

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.Run();
		}
	}
}