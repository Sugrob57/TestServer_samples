using QaTools.WebTests.Core.Abstractions;
using QaTools.WebTests.FrontendAppPages.WebBlocks;

namespace QaTools.WebTests.FrontendAppPages.WebPages
{
	public class IndexWebPage : WebPage
	{
		public IndexWebPage(IWebBrowser browser)
			: base(browser)
		{
		}

		public override bool IsOnPage()
		{
			return ToDoListWebBlock.Displayed;
		}

		public IWebBlock TableTitle => GetWebBlock(By.Css, "#counter").ConfigureAwait(false).GetAwaiter().GetResult();

		public ToDoListWebBlock ToDoListWebBlock => GetWebBlock<ToDoListWebBlock>(By.Css, ".custom-table").ConfigureAwait(false).GetAwaiter().GetResult();
	}
}