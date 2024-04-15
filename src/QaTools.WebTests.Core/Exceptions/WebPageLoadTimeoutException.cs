namespace QaTools.WebTests.Core.Exceptions
{
	public class WebPageLoadTimeoutException : Exception
	{
		public WebPageLoadTimeoutException(string message)
			: base(message)
		{
		}
	}
}