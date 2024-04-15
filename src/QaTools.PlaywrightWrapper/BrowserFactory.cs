using Microsoft.Playwright;
using Serilog;

namespace QaTools.PlaywrightWrapper
{
	public class BrowserFactory
	{
		public static IPlaywright GetPlaywright()
		{
			return PlaywrightLazy.Value;
		}

		public static async Task<IBrowser> GetBrowser(IPlaywright playwright)
		{
			bool headless;

#if DEBUG
			headless = false;
			Log.Logger.Debug("Run mode: Debug");
#else
			headless = true;
			Log.Logger.Debug("Run mode: Release");
#endif


			Log.Debug($"Getting Chrome browser. Headless mode: {headless}.");
			var chromium = playwright.Chromium;

			// Can be "msedge", "chrome-beta", "msedge-beta", "msedge-dev", etc.
			var browser = await chromium.LaunchAsync(new BrowserTypeLaunchOptions { Channel = "chrome", Headless = headless, SlowMo = 50 });

			return browser;
		}

		public static async Task<IPage> GetPageInNewBrowserContext(IBrowser browser)
		{
			Log.Debug("Creating new context.");
			var browserContext = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });

			Log.Debug("Getting new page.");
			return await browserContext.NewPageAsync();
		}

		private static Lazy<IPlaywright> PlaywrightLazy { get; set; }
			= new Lazy<IPlaywright>(Playwright.CreateAsync().ConfigureAwait(false).GetAwaiter().GetResult());
	}
}