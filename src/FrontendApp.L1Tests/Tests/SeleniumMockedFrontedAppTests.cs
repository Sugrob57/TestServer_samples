using FluentAssertions;
using QaTools.SeleniumWrapper.Implementation;
using QaTools.WebTests.FrontendAppPages.PageObjects;
using TestHostLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.MockMappings;
using TodoApp.MockMappings.Mappings;
using Microsoft.Extensions.Configuration;
using Testing.Common.Helpers;
using FrontendAppProgram = ToDo.FrontendApp.Program;

namespace FrontendApp.L1Tests.Tests
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class SeleniumMockedFrontedAppTests : TestBase
	{
		[Test]
		public async Task ExternalWebApplicationFactory_InMemoryWireMock_MockRequestedFromService()
		{
			// arrange
			var todosCount = 15;
			var testTodos = FakeDataGenerators.GetTestTodos(todosCount);

			var mockProvider = new WireMockProvider();
			var mock = mockProvider.GetMock<BackendAppMappings>();
			mock.MockGetTodoList(testTodos);
			mock.MockGetTodoListOptions();


			AppFactory = new CustomWebApplicationFactory<FrontendAppProgram>(services =>
			{
				var configurationDescriptor = services
					.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));

				var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
				configuration["AppSettings:BackendUri"] = $"{mockProvider.ServerAddress}/";
			});

			var endpoint = $"{AppFactory.ServerAddress}Home";
			WebBrowser = new SeleniumWebBrowser();

			// act
			await WebBrowser.GoToUrlAsync(endpoint);
			var page = await WebBrowser.GetPage<IndexPage>();
			var todoList = page.TableRecords;
			var todoTitles = todoList.Select(todo => todo.Title.Text).ToList();

			// assert
			todoList.Count().Should().Be(todosCount);
			todoTitles.Should().BeEquivalentTo(testTodos.Select(todo => todo.Title).ToList());
		}

		[TearDown]
		public void ClassTearDown()
		{
			AppFactory.Dispose();
		}

		private CustomWebApplicationFactory<FrontendAppProgram> AppFactory
		{
			get => NUnitExtensions.GetTestContextProperty<CustomWebApplicationFactory<FrontendAppProgram>>(nameof(AppFactory));
			set => NUnitExtensions.SetTestContextProperty(nameof(AppFactory), value);
		}
	}
}