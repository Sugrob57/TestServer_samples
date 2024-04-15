using ToDo.BackendApp.Models;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace TodoApp.MockMappings.Mappings
{
	public class BackendAppMappings : MockBase
	{
		public BackendAppMappings(WireMockServer mock)
			: base(mock)
		{
		}

		public void MockGetTodoList(List<Todo> todos)
		{
			var request = Request
							.Create()
							.WithPath(new WildcardMatcher($"/api/todo/records", true))
							.UsingGet();

			MockServer
				.Given(request)
				.RespondWith(
					Response
						.Create()
						.WithStatusCode(200)
						.WithBodyAsJson(todos)
						.WithHeaders(_corsHeaders));
		}

		public void MockGetTodoListOptions()
		{
			var request = Request
							.Create()
							.WithPath(new WildcardMatcher($"/api/todo/records", true))
							.UsingOptions();

			MockServer
				.Given(request)
				.RespondWith(
					Response
						.Create()
						.WithStatusCode(200)
						.WithHeaders(_corsHeaders));
		}

		private readonly Dictionary<string, string> _corsHeaders = new()
		{
			{ "Access-Control-Allow-Origin", "*" },
			{ "Access-Control-Allow-Credentials", "true" },
			{ "Access-Control-Allow-Methods", "GET, POST, DELETE, PUT" },
			{ "Access-Control-Allow-Headers", "content-type" }
		};
	}
}