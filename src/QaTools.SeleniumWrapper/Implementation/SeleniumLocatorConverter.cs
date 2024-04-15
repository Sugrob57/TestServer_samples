using QaTools.WebTests.Core.Abstractions;
using SeleniumBy = OpenQA.Selenium.By;

namespace QaTools.SeleniumWrapper.Implementation
{
	internal class SeleniumLocatorConverter
	{
		public static SeleniumBy ToSeleniumBy(By locatorName, string locatorValue)
		{
			if (string.IsNullOrEmpty(locatorValue))
			{
				throw new ArgumentNullException(nameof(locatorValue));
			}

			switch (locatorName)
			{
				case By.Xpath:
					return SeleniumBy.XPath(locatorValue);
				case By.Css:
					return SeleniumBy.CssSelector(locatorValue);
				default:
					throw new ArgumentOutOfRangeException($"{locatorName} is not supported. Use Css or Xpath value");
			}
		}
	}
}
