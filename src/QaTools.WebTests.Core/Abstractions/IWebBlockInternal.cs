namespace QaTools.WebTests.Core.Abstractions
{
	/// <summary>
	/// Internal web block for initialization of IWebBlock instance (used inside Browser search methods)
	/// </summary>
	public interface IWebBlockInternal : IWebBlock
	{
		object WebElement { get; }

		void InitBlock(
			IWebBrowser webBrowser,
			IWebElementImplementation webElement,
			By locatorName,
			string locatorValue,
			IWebBlock parentWebBlock);
	}
}