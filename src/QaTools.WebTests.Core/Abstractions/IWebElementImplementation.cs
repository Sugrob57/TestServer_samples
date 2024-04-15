namespace QaTools.WebTests.Core.Abstractions
{
	/// <summary>
	/// Low-level abstraction, representing adapter between IWebBlock implementaion and native
	/// framework block implementation (as an example IWebElement instance in Selenium Webdriver).
	/// </summary>
	public interface IWebElementImplementation
	{
		object BaseWebElement { get; }

		string Text { get; }

		bool Enabled { get; }

		bool Selected { get; }

		bool Displayed { get; }

		void Clear();

		void Click();

		string GetAttribute(string attributeName);

		void SendKeys(string text);
	}
}