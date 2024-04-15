using QaTools.WebTests.Core.Abstractions;
using QaTools.WebTests.FrontendAppPages.WebBlocks;
using QaTools.WebTests.FrontendAppPages.WebPages;

namespace QaTools.WebTests.FrontendAppPages.PageObjects
{
	public class IndexPage : PageObject<IndexWebPage>
	{
		public string TableTitle => WebPage.TableTitle.Text;

		public IEnumerable<ToDoItemWebBlock> TableRecords => WebPage.ToDoListWebBlock.TableRecords;
	}
}