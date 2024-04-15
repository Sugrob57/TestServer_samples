using QaTools.WebTests.Core.Exceptions;

namespace QaTools.WebTests.Core.Abstractions
{
	public class WebBlock : IWebBlock, IWebBlockInternal
	{
		void IWebBlockInternal.InitBlock(
			IWebBrowser webBrowser,
			IWebElementImplementation webElement,
			By locatorName,
			string locatorValue,
			IWebBlock parentWebBlock)
		{
			_browser = webBrowser;
			_webElement = webElement;
			_locatorName = locatorName;
			_locatorValue = locatorValue;
			_parentWebBlock = parentWebBlock;
		}

		public virtual string Text => WebElement.Text;

		public bool Exists => _webElement is not null;

		public virtual bool Enabled
		{
			get
			{
				return Displayed && WebElement.Enabled && !HasAttribute("disabled");
			}
		}

		public bool Selected => WebElement.Selected;

		public bool Displayed
		{
			get
			{
				return Exists && WebElement.Displayed;
			}
		}

		public void Clear() => WebElement.Clear();

		public virtual void Click() => WebElement.Click();

		public string GetAttribute(string attributeName) => WebElement.GetAttribute(attributeName);

		public void SendKeys(string text) => WebElement.SendKeys(text);

		public bool HasAttribute(string attributeName) => GetAttribute(attributeName) is not null;

		public IWebBrowser Browser => _browser;

		public string FullLocator
		{
			get
			{
				if (_parentWebBlock is null)
				{
					return $"{_locatorName} : {_locatorValue}";
				}
				else
				{
					return $"{_parentWebBlock.FullLocator} {_locatorName} : {_locatorValue}";
				}
			}
		}

		private IWebElementImplementation _webElement;

		private By _locatorName;
		private string _locatorValue;

		object IWebBlockInternal.WebElement => WebElement.BaseWebElement;

		private IWebElementImplementation WebElement =>
			_webElement ??
			throw new NoSuchElementException($"Could not find element with locator {FullLocator}");

		private IWebBlock _parentWebBlock;

		private IWebBrowser _browser;
	}
}