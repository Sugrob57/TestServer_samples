using BackofficeApi.L1Tests;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using TestHostLibrary.Services;
using ToDo.BackendApp;
using ToDo.BackendApp.Models;
using TodoApp.MockMappings;
using TodoApp.MockMappings.Mappings;

namespace BackendApi.L1Tests.Tests
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class UseMockTests : TestBase
	{
		[Test]
		public async Task ExternalWebApplicationFactory_InMemoryWireMock_MockRequestedFromService()
		{
			// arrange
			var newTodo = new Todo()
			{
				Id = 100,
				Title = "Test",
				Description = "Test",
				IsDone = true,
			};

			var email = $"{Guid.NewGuid():N}@todo.com";

			MockServer = new WireMockProvider();
			var emailServiceMock = MockServer.GetMock<MailingAppMappings>();
			emailServiceMock.MockSendEmailRequest(email);

			var factory = new CustomHostFactory<Program>(services =>
			{
				var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
				configuration["AppSettings:SmtpServerAddress"] = MockServer.ServerAddress;
				configuration["AppSettings:NotificationsEmail"] = email;
			});
			WebAppFactory = factory;
			var httpClient = factory.CreateClient();

			// act
			var response = await httpClient.PostAsJsonAsync("api/ToDo/new", newTodo);
			var record = await response.Content.ReadFromJsonAsync<Todo>();

			// assert
			response.IsSuccessStatusCode.Should().BeTrue();
			record.Should().BeEquivalentTo(newTodo);
			MockServer.Server.LogEntries.Where(x => x.RequestMessage.Body.Contains(email)).Should().HaveCount(1);

			MockServer.Dispose();
		}
	}
}