using TodoApp.MockMappings.Mappings;
using WireMock.Server;
using WireMock.Settings;

namespace TodoApp.MockMappings
{
	public class WireMockProvider : IDisposable
	{
		public WireMockProvider()
		{
			Server = WireMockServer.Start(new WireMockServerSettings
			{
				StartAdminInterface = true,
			});
		}

		public T GetMock<T>()
			where T : MockBase
		{
			return (T)Activator.CreateInstance(typeof(T), Server);
		}

		public void Dispose()
		{
			Server.Stop();
		}

		public string ServerAddress => Server.Url ?? Server.Urls.First();

		public WireMockServer Server { get; set; }
	}
}