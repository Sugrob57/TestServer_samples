namespace QaTools.WebTests.Core.Abstractions
{
	public abstract class PageObject<TWebPage>
		: IPage, IPageInternal
		where TWebPage : WebPage
	{
		protected PageObject()
		{
		}

		void IPageInternal.SetWebPage(WebPage webPage)
		{
			WebPage = webPage as TWebPage;
		}

		public IWebBrowser Browser =>
			WebPage?.Browser ?? throw new NullReferenceException($"Web page {GetType()} was not initialized");

		Type IPageInternal.WebPageType => typeof(TWebPage);

		public bool IsOnPage() => WebPage.IsOnPage();

		protected TWebPage WebPage { get; private set; }
	}
}