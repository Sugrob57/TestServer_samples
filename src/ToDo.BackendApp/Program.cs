using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ToDo.BackendApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IConfiguration configuration = null;

			var builder = Host
				.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostingContext, configBuilder) =>
				{
					configBuilder.Sources.Clear();
					configBuilder.Properties.Clear();
					configuration = configBuilder
						.AddJsonFile("commonAppSettings.json")
						.AddEnvironmentVariables()
						.AddCommandLine(args)
						.Build();
				});

			builder
				.ConfigureWebHost(webBuilder =>
				{
					webBuilder
						.UseKestrel(options => 
						{
							var httpPort = 0;
							webBuilder.ConfigureServices(services =>
							{
								httpPort = int.Parse(services
									.BuildServiceProvider()
									.GetService<IConfiguration>()
									.GetSection("WebHostSettings:BackendAppPort").Value);
							});

							options.AddServerHeader = false;
							options.Limits.MaxConcurrentConnections = 100;

							options.Listen(IPAddress.Any, httpPort, listenOptions =>
							{
								listenOptions.UseHttps();
							});
						})
						.UseStartup<Startup>();
				})
				.Build()
				.Run();
		}
	}
}