namespace QaTools.WebTests.Core.Abstractions
{
	public interface IPage
	{
		IWebBrowser Browser { get; }

		bool IsOnPage();
	}
}