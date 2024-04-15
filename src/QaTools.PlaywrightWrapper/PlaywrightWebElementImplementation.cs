using Microsoft.Playwright;
using QaTools.WebTests.Core.Abstractions;

namespace QaTools.PlaywrightWrapper
{
	internal class PlaywrightWebElementImplementation : IWebElementImplementation
	{
		public PlaywrightWebElementImplementation(ILocator locator)
		{
			_locator = locator;
		}

		public static PlaywrightWebElementImplementation GetNewInstance(ILocator locator)
		{
			if (locator is not null)
			{
				return new PlaywrightWebElementImplementation(locator);
			}

			return null;
		}

		public object BaseWebElement => _locator;

		public string Text
		{
			get
			{
				var text = CallWebElement<string>(() => _locator.InnerTextAsync());
				return text.Contains("\t") ? text?.Split("\t")?.Skip(1).FirstOrDefault() : text;
			}
		}

		public bool Enabled => CallWebElement<bool>(() => _locator.IsEnabledAsync());

		public bool Selected => CallWebElement<bool>(() => _locator.IsCheckedAsync());

		public bool Displayed => CallWebElement<bool>(() => _locator.IsVisibleAsync());

		public void Clear() => CallWebElement(() => _locator.ClearAsync());

		public void Click() => CallWebElement(() => _locator.ClickAsync());

		public string GetAttribute(string attributeName) => CallWebElement<string>(() => _locator.GetAttributeAsync(attributeName));

		public void SendKeys(string text) =>
			_locator.FillAsync(text).ConfigureAwait(false).GetAwaiter().GetResult();

		private T CallWebElement<T>(Func<Task<T>> actionCall)
		{
			return actionCall().ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private void CallWebElement(Func<Task> actionCall)
		{
			actionCall().ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private readonly ILocator _locator;
	}
}