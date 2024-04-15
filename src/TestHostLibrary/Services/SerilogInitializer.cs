using Serilog.Events;
using Serilog;

namespace TestHostLibrary.Services
{
	public class SerilogInitializer
	{
		public static void CreateLogger()
		{
			var configuration = new LoggerConfiguration()
				.MinimumLevel.Is(LogEventLevel.Debug)
				.WriteTo.NUnitOutput();

			Log.Logger = configuration
				.CreateLogger();
		}
	}
}