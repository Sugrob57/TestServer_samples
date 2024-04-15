using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace TestHostLibrary.Services
{
	public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
		where TStartup : class
	{
		public CustomWebApplicationFactory(Action<IServiceCollection> configureServices = null)
		{
			_configureServices = configureServices;
		}

		protected override IHost CreateHost(IHostBuilder builder)
		{
			var testHost = builder.Build();

			builder
				.ConfigureWebHost(webHostBuilder => 
				{
					webHostBuilder.UseKestrel();
				});


			_host = builder.Build();
			_host.Start();

			var server = _host.Services.GetRequiredService<IServer>();
			var addresses = server.Features.Get<IServerAddressesFeature>();

			ClientOptions.BaseAddress = addresses!.Addresses
				.Select(x => new Uri(x))
				.Last();

			testHost.Start();
			return testHost;
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			if (_configureServices is not null)
			{
				builder.ConfigureServices(_configureServices);
			}

			builder.ConfigureServices(services =>
			{
				Log.Debug("Test ConfigureServices");
			});

			builder.UseKestrel();
			builder.UseSetting("urls", "http://0.0.0.0:0;https://0.0.0.0:0");
			builder.UseEnvironment("Development");
		}

		public new void Dispose()
		{
			base.Dispose();
			_host.Dispose();
		}

		public string ServerAddress
		{
			get
			{
				EnsureServer();
				return ClientOptions.BaseAddress
					.ToString()
					.Replace("[::]", "localhost")
					.Replace("0.0.0.0", "localhost");
			}
		}

		private void EnsureServer()
		{
			if (_host is null)
			{
				// This forces WebApplicationFactory to bootstrap the server.
				using var _ = CreateDefaultClient();
			}
		}

		private readonly Action<IServiceCollection> _configureServices;

		private IHost _host;
	}
}