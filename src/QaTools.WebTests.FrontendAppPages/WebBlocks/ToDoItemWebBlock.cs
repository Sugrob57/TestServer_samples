using QaTools.WebTests.Core.Abstractions;

namespace QaTools.WebTests.FrontendAppPages.WebBlocks
{
	public class ToDoItemWebBlock : SearchableWebBlock
	{
		public override bool Enabled
		{
			get
			{
				return base.Enabled;
			}
		}

		public IWebBlock IsDone => GetWebBlock(By.Css, "#isDone").ConfigureAwait(false).GetAwaiter().GetResult();

		public IWebBlock Title => GetWebBlock(By.Css, "#title").ConfigureAwait(false).GetAwaiter().GetResult();

		public IWebBlock Edit => GetWebBlock(By.Css, "#edit").ConfigureAwait(false).GetAwaiter().GetResult();

		public IWebBlock Delete => GetWebBlock(By.Css, "#delete").ConfigureAwait(false).GetAwaiter().GetResult();
	}
}