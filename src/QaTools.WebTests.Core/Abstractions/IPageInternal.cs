namespace QaTools.WebTests.Core.Abstractions
{
	public interface IPageInternal
		: IPage
	{
		Type WebPageType { get; }

		void SetWebPage(WebPage webPage);
	}
}