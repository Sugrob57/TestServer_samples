namespace QaTools.WebTests.Core.Abstractions
{
	public abstract class SearchableWebBlock : WebBlock, ISearchable
	{
		public async Task<IWebBlock> GetWebBlock(
			By locatorName, string locatorValue, TimeSpan? waitTimeout = null) =>
			await Browser.GetElementFromElementBy(this, locatorName, locatorValue, waitTimeout);

		public async Task<IEnumerable<IWebBlock>> GetWebBlocks(
			By locatorName, string locatorValue, TimeSpan? waitTimeout = null) =>
			await GetWebBlocks<WebBlock>(locatorName, locatorValue, waitTimeout);

		public async Task<T> GetWebBlock<T>(By locatorName, string locatorValue, TimeSpan? waitTimeout = null)
			where T : class, IWebBlock =>
			await Browser.GetElementFromElementBy<T>(this, locatorName, locatorValue, waitTimeout);

		public async Task<IEnumerable<T>> GetWebBlocks<T>(
			By locatorName, string locatorValue, TimeSpan? waitTimeout = null)
			where T : class, IWebBlock =>
			await Browser.GetElementsFromElementBy<T>(this, locatorName, locatorValue, waitTimeout);
	}
}