namespace QaTools.WebTests.Core.Abstractions
{
	public abstract class WebPage : ISearchable, IPage
	{
		protected WebPage(IWebBrowser browser)
		{
			Browser = browser;
		}

		public abstract bool IsOnPage();

		public async Task<T> GetWebBlock<T>(By locatorName, string locatorValue, TimeSpan? waitTimeout = null)
			where T : class, IWebBlock => await Browser.GetElementBy<T>(locatorName, locatorValue, waitTimeout);

		public async Task<IEnumerable<T>> GetWebBlocks<T>(By locatorName, string locatorValue, TimeSpan? waitTimeout = null)
			where T : class, IWebBlock => await Browser.GetElementsBy<T>(locatorName, locatorValue, waitTimeout);

		public async Task<IWebBlock> GetWebBlock(By locatorName, string locatorValue, TimeSpan? waitTimeout = null) =>
			await Browser.GetElementBy(locatorName, locatorValue, waitTimeout);

		public async Task<IEnumerable<IWebBlock>>  GetWebBlocks(By locatorName, string locatorValue, TimeSpan? waitTimeout = null) =>
			await Browser.GetElementsBy(locatorName, locatorValue, waitTimeout);

		public IWebBrowser Browser { get; }
	}
}