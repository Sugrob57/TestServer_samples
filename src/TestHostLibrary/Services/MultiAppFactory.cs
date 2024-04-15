using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestHostLibrary.Services
{
	public class MultiAppFactory<TFrontendApp, TBackendApp> : IDisposable
		where TFrontendApp : class
		where TBackendApp : class
	{
		public MultiAppFactory()
		{
			BackendFactory = new CustomHostFactory<TBackendApp>();
			BackendAddress = BackendFactory.ServerAddress;

			FrontendFactory = new CustomWebApplicationFactory<TFrontendApp>(services =>
			{
				var configurationDescriptor = services
					.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));

				var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
				configuration["AppSettings:BackendUri"] = BackendAddress;
			});

			FrontendAddress = FrontendFactory.ServerAddress;
		}

		public CustomHostFactory<TBackendApp> BackendFactory { get; private set; }

		public CustomWebApplicationFactory<TFrontendApp> FrontendFactory { get; private set; }

		public string BackendAddress { get; private set; }

		public string FrontendAddress { get; private set; }

		public void Dispose()
		{
			FrontendFactory.Dispose();
			BackendFactory.Dispose();
		}
	}
}