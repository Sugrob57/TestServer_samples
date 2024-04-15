using OpenQA.Selenium;
using QaTools.WebTests.Core.Abstractions;
using QaTools.WebTests.Core.Exceptions;

namespace QaTools.SeleniumWrapper.Implementation
{
	internal class SeleniumWebElementImplementation : IWebElementImplementation
	{
		public SeleniumWebElementImplementation(IWebElement webElement)
		{
			_webElement = webElement;
		}

		public static SeleniumWebElementImplementation GetNewInstance(IWebElement webElement)
		{
			if (webElement is not null)
			{
				return new SeleniumWebElementImplementation(webElement);
			}

			return null;
		}

		public object BaseWebElement => _webElement;

		public string Text => CallWebElement(() => _webElement.Text);

		public bool Enabled => CallWebElement(() => _webElement.Enabled);

		public bool Selected => CallWebElement(() => _webElement.Selected);

		public bool Displayed => CallWebElement(() => _webElement.Displayed);

		public void Clear() => CallWebElement(() => _webElement.Clear());

		public void Click() => CallWebElement(() => _webElement.Click());

		public string GetAttribute(string attributeName) =>
			CallWebElement(() => _webElement.GetAttribute(attributeName));

		public void SendKeys(string text) =>
			CallWebElement(() => _webElement.SendKeys(text));

		private T CallWebElement<T>(Func<T> actionCall)
		{
			try
			{
				return actionCall();
			}
			catch (StaleElementReferenceException ex)
			{
				throw new StaleElementException(ex.Message);
			}
		}

		private void CallWebElement(Action actionCall)
		{
			try
			{
				actionCall();
			}
			catch (StaleElementReferenceException ex)
			{
				throw new StaleElementException(ex.Message);
			}
		}

		private readonly IWebElement _webElement;
	}
}