using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TestHostLibrary.Services;
using Testing.Common.HttpClient;
using ToDo.FrontendApp;

namespace FrontendApp.L1Tests.Tests
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class WebApplicationFactoryInitTests
	{
		[Test]
		public async Task DefaultWebApplicationFactory_InternalHttpRequestToService_Success()
		{
			// arrange
			var factory = new WebApplicationFactory<Program>();
			var httpClient = factory.CreateClient();

			// act
			var htmlPageResponse = await httpClient.GetStringAsync("Home");

			// assert
			htmlPageResponse.Should().Contain("navbarSupportedContent");

			// dispose
			factory.Dispose();
		}

		[Test]
		public async Task DefaultWebApplicationFactory_SetBaseUrl_InternalHttpRequestToService_Success()
		{
			// arrange
			WebApplicationFactory = new WebApplicationFactory<Program>();
			var httpClient = WebApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
			{
				AllowAutoRedirect = true,
				BaseAddress = new Uri("https://localhost:9999")
			});

			// act
			var endpoint = "https://localhost:9999/Home";
			var htmlPageResponse = await httpClient.GetStringAsync(endpoint);

			// assert
			htmlPageResponse.Should().Contain("todos");
		}

		[Test]
		public void DefaultWebApplicationFactory_TrySendExternalHttpRequestToService_Failed()
		{
			// arrange
			WebApplicationFactory = new WebApplicationFactory<Program>();
			var internalClient = WebApplicationFactory.CreateClient();
			var httpClient = new RestHttpClient();

			// act
			var endpoint = $"{internalClient.BaseAddress}Home";
			Action act = () => httpClient.Get(endpoint);

			// assert
			endpoint.Should().Be($"http://localhost/Home");
			act.Should().Throw<RestHttpClientException>();
		}

		[Test]
		public void CustomWebApplicationFactory_ExternalHttpRequestToService_Success()
		{
			// arrange
			var extFactory = new CustomWebApplicationFactory<Program>();
			WebApplicationFactory = extFactory;
			var baseAddress = extFactory.ServerAddress;
			var httpClient = new RestHttpClient();

			// act
			var endpoint = $"{baseAddress}Home";
			var htmlPageResponse = httpClient.Get(endpoint);

			// assert
			endpoint.Should().StartWith($"https://localhost:").And.EndWith("/Home");
			htmlPageResponse.Should().Contain("todos");
		}

		[TearDown]
		public void TestTearDown()
		{
			WebApplicationFactory?.Dispose();
		}

		private WebApplicationFactory<Program> WebApplicationFactory
		{
			get => NUnitExtensions.GetTestContextProperty<WebApplicationFactory<Program>>(nameof(WebApplicationFactory));
			set => NUnitExtensions.SetTestContextProperty(nameof(WebApplicationFactory), value);
		}
	}
}