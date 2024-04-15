using Bogus;
using Serilog;
namespace TestHostLibrary.Services
{
	public class PortGenerator
	{
		public static int GetFreePort()
		{
			var faker = new Faker();
			var port = 0;

			do
			{
				port = faker.Random.Number(10000, 64000);
			}
			while (_usedPorts.Contains(port));

			_usedPorts.Add(port);
			Log.Debug("Port {port}", port);
			return port;
		}

		private static List<int> _usedPorts = new();
	}
}