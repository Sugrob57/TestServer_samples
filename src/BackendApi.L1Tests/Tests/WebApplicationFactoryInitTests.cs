using BackofficeApi.L1Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TestHostLibrary.Services;
using Testing.Common;
using Testing.Common.HttpClient;
using ToDo.BackendApp;
using ToDo.BackendApp.Models;

namespace BackendApi.L1Tests.Tests
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class WebApplicationFactoryInitTests : TestBase
	{
		[Test]
		public async Task DefaultWebApplicationFactory_InternalHttpRequestToService_Success()
		{
			// arrange
			WebAppFactory = new WebApplicationFactory<Program>();
			var httpClient = WebAppFactory.CreateClient();

			// act
			var response = await httpClient.GetStringAsync("api/ToDo/records");
			var records = response.FromJson<List<Todo>>();

			// assert
			records.Count.Should().BeGreaterThan(0);
		}

		[Test]
		public async Task DefaultWebApplicationFactory_SetBaseUrl_InternalHttpRequestToService_Success()
		{
			// arrange
			WebAppFactory = new WebApplicationFactory<Program>();
			var httpClient = WebAppFactory.CreateClient(new WebApplicationFactoryClientOptions
			{
				AllowAutoRedirect = true,
				BaseAddress = new Uri("https://localhost:9999")
			});

			// act
			var endpoint = "https://localhost:9999/api/ToDo/records";
			var response = await httpClient.GetStringAsync(endpoint);
			var records = response.FromJson<List<Todo>>();

			// assert
			records.Count.Should().BeGreaterThan(0);
		}

		[Test]
		public void DefaultWebApplicationFactory_TrySendExternalHttpRequestToService_Failed()
		{
			// arrange
			WebAppFactory = new WebApplicationFactory<Program>();
			var internalClient = WebAppFactory.CreateClient();
			var httpClient = new RestHttpClient();

			// act
			var endpoint = $"{internalClient.BaseAddress}api/ToDo/records";
			Action act = () => httpClient.Get<List<Todo>>(endpoint);

			// assert
			endpoint.Should().Be($"http://localhost/api/ToDo/records");
			act.Should().Throw<RestHttpClientException>();
		}

		[Test]
		public void CustomWebApplicationFactory_ExternalHttpRequestToService_Success()
		{
			// arrange
			var factory = new CustomHostFactory<Program>();
			WebAppFactory = factory;
			var baseAddress = factory.ServerAddress;
			var httpClient = new RestHttpClient();

			// act
			var endpoint = $"{baseAddress}api/ToDo/records";
			var records = httpClient.Get<List<Todo>>(endpoint);

			// assert
			endpoint.Should().StartWith($"https://localhost:").And.EndWith("/api/ToDo/records");
			records.Count.Should().BeGreaterThan(0);
		}
	}
}