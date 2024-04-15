using WireMock.Server;

namespace TodoApp.MockMappings.Mappings
{
	public abstract class MockBase
	{
		protected MockBase(WireMockServer mockServer)
		{
			MockServer = mockServer;
		}

		public void Clear()
		{
			MockServer.ResetLogEntries();
			MockServer.ResetMappings();
			MockServer.ResetScenarios();
			MockServer.Reset();
		}

		protected WireMockServer MockServer { get; private set; }
	}
}