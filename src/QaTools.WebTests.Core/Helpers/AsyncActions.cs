using QaTools.WebTests.Core.Exceptions;
using Serilog;
using System.Diagnostics;

namespace QaTools.WebTests.Core.Helpers
{
	public class AsyncActions
	{
		public static async Task WaitForExternalEventAsync(
			Func<Task<bool>> eventTriggeredPredicate,
			TimeSpan? timeout = null,
			int completedCheckIntervalInSeconds = 1,
			string timeoutExceptionMessage = null)
		{
			timeout = timeout ?? TimeSpan.FromSeconds(10.0);
			Log.Debug("Started external event sync waiting");
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (stopwatch.Elapsed.Ticks < timeout.Value.Ticks)
			{
				if (await eventTriggeredPredicate.Invoke())
				{
					Log.Debug($"Event {eventTriggeredPredicate.Method.Name} was triggered in: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
					return;
				}

				await Task.Delay(TimeSpan.FromSeconds(completedCheckIntervalInSeconds));
			}

			timeoutExceptionMessage = timeoutExceptionMessage ?? $"Timeout of {timeout} was reached while waiting for external event to trigger in {eventTriggeredPredicate.Method.Name}";
			Log.Error(timeoutExceptionMessage);
			throw new WebPageLoadTimeoutException(timeoutExceptionMessage);
		}
	}
}