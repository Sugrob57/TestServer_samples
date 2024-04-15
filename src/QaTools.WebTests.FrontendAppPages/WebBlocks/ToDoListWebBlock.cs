using QaTools.WebTests.Core.Abstractions;

namespace QaTools.WebTests.FrontendAppPages.WebBlocks
{
	public class ToDoListWebBlock : SearchableWebBlock
	{
		public override bool Enabled
		{
			get
			{
				return base.Enabled && TableRecords.Any();
			}
		}


		public async Task<IEnumerable<ToDoItemWebBlock>> TableRecords2() => await GetWebBlocks<ToDoItemWebBlock>(By.Css, "#todos tr");

		public IEnumerable<ToDoItemWebBlock> TableRecords => GetWebBlocks<ToDoItemWebBlock>(By.Css, "#todos tr").ConfigureAwait(false).GetAwaiter().GetResult();
	}
}