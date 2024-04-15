using System.Drawing;

namespace QaTools.WebTests.Core.Abstractions
{
	public interface IWebBrowser : IDisposable
	{
		string Url { get; }

		string Title { get; }

		public Task<TPage> GetPage<TPage>()
			where TPage : class, IPage;

		Task RefreshAsync();

		Task BackAsync();

		Task GoToUrlAsync(string url);

		Task<string> GetScreenshotAsync(string directory, string fileName);

		Task MaximizeWindowAsync();

		Task<IWebBlock> GetElementBy(By locatorName, string locatorValue, TimeSpan? timeout = null);

		Task<T> GetElementBy<T>(By locatorName, string locatorValue, TimeSpan? timeout = null)
			 where T : class, IWebBlock;

		Task<IEnumerable<T>> GetElementsBy<T>(
			By locatorName, string locatorValue, TimeSpan? timeout = null, IWebBlock searchContext = null)
			where T : class, IWebBlock;

		Task<IEnumerable<IWebBlock>> GetElementsBy(By locatorName, string locatorValue, TimeSpan? timeout = null);

		Task<IWebBlock> GetElementFromElementBy(
			IWebBlock anotherElement, By locatorName, string locatorValue, TimeSpan? timeout = null);

		Task<T> GetElementFromElementBy<T>(
			IWebBlock anotherElement, By locatorName, string locatorValue, TimeSpan? timeout = null)
			where T : class, IWebBlock;

		Task<IEnumerable<T>> GetElementsFromElementBy<T>(
			IWebBlock anotherElement, By locatorName, string locatorValue, TimeSpan? waitTimeout = null)
			where T : class, IWebBlock;
	}
}