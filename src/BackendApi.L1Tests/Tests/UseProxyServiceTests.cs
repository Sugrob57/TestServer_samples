using BackendApi.L1Tests.Fixtures;
using BackofficeApi.L1Tests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using TestHostLibrary.Services;
using Testing.Common;
using ToDo.BackendApp;
using ToDo.BackendApp.Models;
using ToDo.BackendApp.Services;
using FluentAssertions.Execution;

namespace BackendApi.L1Tests.Tests
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class UseProxyServiceTests : TestBase
	{
		[Test]
		public async Task NotSingletonService_UseProxyService_InterceptOnlyOneRequest_OnlyInterceptedRequestIsPassed()
		{
			// arrange
			var factory = new CustomHostFactory<Program>(services =>
			{
				var todoServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITodoService));

				services.Remove(todoServiceDescriptor);
				services.AddTransient<ITodoService, ToDoServiceProxy>();
				services.AddTransient<TodoService>();
			});

			WebAppFactory = factory;
			var httpClient = factory.CreateClient();

			// act
			var response = await httpClient.GetStringAsync("api/ToDo/records");
			var records = response.FromJson<List<Todo>>();

			var brokenTodo = records.First();
			var successTodo = records.Last();
			TodoServiceInterceptActions.Add(brokenTodo.Id, () => throw new Exception("I am broken todo record"));

			var failedRequest = await httpClient.PutAsJsonAsync($"api/ToDo/{brokenTodo.Id}", brokenTodo);
			var successRequest = await httpClient.PutAsJsonAsync($"api/ToDo/{successTodo.Id}", successTodo);

			// assert
			using (new AssertionScope())
			{
				failedRequest.IsSuccessStatusCode.Should().BeFalse("todo from this action is broken");
				successRequest.IsSuccessStatusCode.Should().BeTrue("todo from this action not broken");
			}
		}

		public static Dictionary<int, Action> TodoServiceInterceptActions = new Dictionary<int, Action>();
	}
}