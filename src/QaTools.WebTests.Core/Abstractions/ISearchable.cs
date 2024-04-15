namespace QaTools.WebTests.Core.Abstractions
{
	public interface ISearchable
	{
		Task<T> GetWebBlock<T>(By locatorName, string locatorValue, TimeSpan? waitTimeout = null)
			where T : class, IWebBlock;

		Task<IEnumerable<T>> GetWebBlocks<T>(
			By locatorName, string locatorValue, TimeSpan? waitTimeout = null)
			where T : class, IWebBlock;

		Task<IWebBlock> GetWebBlock(
			By locatorName, string locatorValue, TimeSpan? waitTimeout = null);

		Task<IEnumerable<IWebBlock>> GetWebBlocks(
			By locatorName, string locatorValue, TimeSpan? waitTimeout = null);
	}
}