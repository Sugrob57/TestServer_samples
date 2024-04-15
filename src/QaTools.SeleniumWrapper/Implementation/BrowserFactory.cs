using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Net;
using Serilog;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace QaTools.SeleniumWrapper.Implementation
{
	public class BrowserFactory
	{
		public static IWebDriver GetWebDriver(
			List<string> browserOptions = null,
			bool replaceDefaultBrowserOptions = false)
		{
			return GetGoogleChromeDriver(
				browserOptions,
				replaceDefaultBrowserOptions);
		}
		private static IWebDriver GetGoogleChromeDriver(
			List<string> options = null,
			bool replaceDefaultOptions = false)
		{
			var chromeOptions = new ChromeOptions
			{
				PageLoadStrategy = PageLoadStrategy.Normal,
			};

			if (!replaceDefaultOptions)
			{
				chromeOptions.AddArgument("no-sandbox");
				chromeOptions.AddArgument("--lang=en");
				chromeOptions.AddArgument("--ignore-certificate-errors");
			}

			if (options != null)
			{
				foreach (var option in options)
				{
					chromeOptions.AddArgument(option);
				}
			}
#if DEBUG
			Log.Logger.Debug("Run mode: Debug");
#else
            if (!forceNonHeadlessMode)
            {
                chromeOptions.AddArgument("--headless");
            }

            Log.Logger.Debug($"Run mode: Release, forceNonHeadlessMode: {forceNonHeadlessMode}");
#endif

			IWebDriver driver;

			Log.Logger.Debug("Get chrome driver binary file");

			new DriverManager()
				.WithProxy(WebRequest.GetSystemWebProxy())
				.SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
			var chromeDriverService = ChromeDriverService.CreateDefaultService();

			Log.Logger.Debug($"Create chrome driver session with parameters:\n"
							 + $"ServiceUrl: {chromeDriverService.ServiceUrl}\n"
							 + $"DriverPath: {chromeDriverService.DriverServicePath}\n");

			driver = new ChromeDriver(chromeDriverService, chromeOptions, TimeSpan.FromMinutes(2));
			driver.Manage().Window.Maximize();

			return driver;
		}
	}
}