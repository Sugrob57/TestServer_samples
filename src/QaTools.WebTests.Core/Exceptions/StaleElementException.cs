namespace QaTools.WebTests.Core.Exceptions
{
	public class StaleElementException : Exception
	{
		public StaleElementException(string message)
			: base(message)
		{
		}
	}
}