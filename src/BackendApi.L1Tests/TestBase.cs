using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using TestHostLibrary.Services;
using ToDo.BackendApp;
using TodoApp.MockMappings;

namespace BackofficeApi.L1Tests
{
	[Parallelizable(ParallelScope.All)]
	public abstract class TestBase
	{
		[OneTimeSetUp]
		public void BaseOneTimeSetup()
		{
			SerilogInitializer.CreateLogger();

			Configuration = new ConfigurationBuilder()
				.AddJsonFile("commonAppSettings.json", true, true)
				.Build();
		}

		[TearDown]
		public void BaseTearDown()
		{
			WebAppFactory?.Dispose();
			MockServer?.Dispose();
		}

		protected WebApplicationFactory<Program> WebAppFactory
		{
			get => NUnitExtensions.GetTestContextProperty<WebApplicationFactory<Program>>(nameof(WebAppFactory));
			set => NUnitExtensions.SetTestContextProperty(nameof(WebAppFactory), value);
		}

		protected WireMockProvider MockServer
		{
			get => NUnitExtensions.GetTestContextProperty<WireMockProvider>(nameof(MockServer));
			set => NUnitExtensions.SetTestContextProperty(nameof(MockServer), value);
		}

		protected IConfiguration Configuration { get; set; }
	}
}