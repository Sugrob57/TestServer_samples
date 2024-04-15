using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace TodoApp.MockMappings.Mappings
{
	public class MailingAppMappings : MockBase
	{
		public MailingAppMappings(WireMockServer mock)
			: base(mock)
		{
		}

		public void MockSendEmailRequest(string sendTo = null)
		{
			var request = Request
						.Create()
						.WithPath(new WildcardMatcher($"/api/email", true))
						.UsingPost();

			if (sendTo != null)
			{
				request.WithBody(new JmesPathMatcher($"to == '{sendTo}'"));
			}

			MockServer
				.Given(request)
				.RespondWith(
					Response
						.Create()
						.WithStatusCode(201)
						.WithBodyAsJson(new
						{
							Status = "Success"
						})
						.WithHeaders(_corsHeaders));
		}

		private readonly Dictionary<string, string> _corsHeaders = new()
		{
			{ "Access-Control-Allow-Origin", "*" },
			{ "Access-Control-Allow-Credentials", "true" },
			{ "Access-Control-Allow-Methods", "GET, POST, DELETE, OPTIONS" }
		};
	}
}