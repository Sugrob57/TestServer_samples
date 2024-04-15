namespace QaTools.WebTests.Core.Exceptions
{
	public class NoSuchElementException : Exception
	{
		public NoSuchElementException(string message)
			: base(message)
		{
		}
	}
}