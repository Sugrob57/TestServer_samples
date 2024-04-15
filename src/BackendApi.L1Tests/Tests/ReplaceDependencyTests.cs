using BackendApi.L1Tests.Fixtures;
using BackofficeApi.L1Tests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TestHostLibrary.Services;
using Testing.Common;
using Testing.Common.Helpers;
using ToDo.BackendApp;
using ToDo.BackendApp.Models;
using ToDo.BackendApp.Services;

namespace BackendApi.L1Tests.Tests
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ReplaceDependencyTests : TestBase
	{
		[Test]
		public async Task DefaultWebApplicationFactory_InternalHttpRequestToService_Success()
		{
			// arrange
			var todosCount = 15;
			var testTodos = FakeDataGenerators.GetTestTodos(todosCount);

			var factory = new CustomHostFactory<Program>(services =>
			{
				var dbContextDescriptor = services
					.SingleOrDefault(d => d.ServiceType == typeof(IStorageContext<Todo>));

				services.Remove(dbContextDescriptor);
				services.AddSingleton<IStorageContext<Todo>>(new TodoContextMock(testTodos));
			});

			WebAppFactory = factory;
			var httpClient = factory.CreateClient();

			// act
			var response = await httpClient.GetStringAsync("api/ToDo/records");
			var records = response.FromJson<List<Todo>>();

			// assert
			records.Should().HaveCount(todosCount);
			records.Should().BeEquivalentTo(testTodos);
		}
	}
}