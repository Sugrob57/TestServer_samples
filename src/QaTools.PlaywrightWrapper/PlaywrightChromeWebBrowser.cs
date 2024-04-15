using Microsoft.Playwright;
using QaTools.WebTests.Core.Abstractions;
using Serilog;
using IInternalPage = Microsoft.Playwright.IPage;
using IPage = QaTools.WebTests.Core.Abstractions.IPage;


namespace QaTools.PlaywrightWrapper
{
	public class PlaywrightChromeWebBrowser : IWebBrowser, IDisposable
	{
		public PlaywrightChromeWebBrowser()
		{
		}

		public async Task<PlaywrightChromeWebBrowser> InitAsync()
		{
			if (Playwright == null)
			{
				Log.Debug("Start initialize chrome browser");
				Playwright = BrowserFactory.GetPlaywright();
				Browser = await BrowserFactory.GetBrowser(Playwright);
				Page = await BrowserFactory.GetPageInNewBrowserContext(Browser);
			}

			return this;
		}

		public async Task<TPage> GetPage<TPage>()
			where TPage : class, IPage
		{
			await Page.WaitForLoadStateAsync(LoadState.Load);
			await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions { Timeout = (float)TimeSpan.FromSeconds(30).TotalMilliseconds });

			var page = (TPage)Activator.CreateInstance(typeof(TPage)) as IPageInternal;
			var webPage = (WebPage)Activator.CreateInstance(page.WebPageType, this);
			page.SetWebPage(webPage);

			return page as TPage;
		}

		public string Url => Page.Url;

		public string Title => Page.TitleAsync().ConfigureAwait(false).GetAwaiter().GetResult();

		public async Task RefreshAsync()
		{
			Log.Debug("Refresh page.");
			await Page.ReloadAsync();

			try
			{
				Page.Dialog += async (_, dialog) =>
				{
					await dialog.AcceptAsync();
				};
				await Page.GetByRole(AriaRole.Button).ClickAsync();
			}
			catch (Exception ex)
			{
				Log.Verbose("Any alerts not displayed");
			}
		}

		public async Task BackAsync()
		{
			Log.Debug("Back page.");
			await Page.GoBackAsync();
			Log.Debug($"CurrentUrl: {Url}");
		}

		public async Task GoToUrlAsync(string url)
		{
			try
			{
				await Page.GotoAsync(url);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Timeout occured while going to url {url}.", url);
				throw;
			}
		}

		public async Task<string> GetScreenshotAsync(string directory, string fileName)
		{
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var filePath = GetFilePath(directory, fileName);
			Log.Debug($"Getting WebDriver screenshot to {filePath}");
			await Page.ScreenshotAsync(new PageScreenshotOptions { Path = filePath });
			return filePath;
		}

		public async Task MaximizeWindowAsync()
		{
			await Page.SetViewportSizeAsync(1920, 1080);
		}

		public void Dispose()
		{
			if (Browser != null)
			{
				Browser.DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();
				Browser = null;
				Page = null;
			}
		}

		public async Task<IWebBlock> GetElementBy(
			By locatorName, string locatorValue, TimeSpan? timeout = null) =>
			await InitWebBlock(locatorName, locatorValue, timeout);

		public async Task<T> GetElementBy<T>(
			By locatorName, string locatorValue, TimeSpan? timeout = null)
			where T : class, IWebBlock =>
			await InitWebBlock<T>(locatorName, locatorValue, timeout);

		public async Task<IEnumerable<IWebBlock>> GetElementsBy(
			By locatorName, string locatorValue, TimeSpan? timeout = null) =>
			await InitWebBlocks<WebBlock>(locatorName, locatorValue, timeout);

		public async Task<IEnumerable<T>> GetElementsBy<T>(
			By locatorName, string locatorValue, TimeSpan? timeout = null, IWebBlock searchContext = null)
			where T : class, IWebBlock =>
			await InitWebBlocks<T>(locatorName, locatorValue, timeout);

		public async Task<IWebBlock> GetElementFromElementBy(
			IWebBlock anotherElement, By locatorName, string locatorValue, TimeSpan? timeout = null) =>
			await InitWebBlock(locatorName, locatorValue, timeout, anotherElement);

		public async Task<T> GetElementFromElementBy<T>(
			IWebBlock anotherElement, By locatorName, string locatorValue, TimeSpan? timeout = null)
			where T : class, IWebBlock =>
			await InitWebBlock<T>(locatorName, locatorValue, timeout, anotherElement);

		public async Task<IEnumerable<T>> GetElementsFromElementBy<T>(
			IWebBlock anotherElement, By locatorName, string locatorValue, TimeSpan? waitTimeout = null)
			where T : class, IWebBlock =>
			await InitWebBlocks<T>(locatorName, locatorValue, waitTimeout, anotherElement);

		private async Task<WebBlock> InitWebBlock(
			By locatorName,
			string locatorValue,
			TimeSpan? timeout = null,
			IWebBlock elementContext = null) =>
			await InitWebBlock<WebBlock>(locatorName, locatorValue, timeout, elementContext);

		private async Task<T> InitWebBlock<T>(
			By locatorName,
			string locatorValue,
			TimeSpan? timeout = null,
			IWebBlock elementContext = null)
			where T : class, IWebBlock
		{
			ILocator element = null;

			Log.Verbose($"Wait element by {locatorName} with path '{locatorValue}'");

			element = await FindElement(locatorName, locatorValue, timeout, elementContext);
			Log.Verbose($"Found element by {locatorName} with path '{locatorValue}'");
			var elementInstance = Activator.CreateInstance(typeof(T)) as IWebBlockInternal;

			elementInstance.InitBlock(
				this,
				PlaywrightWebElementImplementation.GetNewInstance(element),
				locatorName,
				locatorValue,
				elementContext);

			return (T)elementInstance;
		}

		public async Task<IEnumerable<T>> InitWebBlocks<T>(
			By locatorName, string locatorValue, TimeSpan? timeout = null, IWebBlock searchContext = null)
			where T : class, IWebBlock
		{
			var searchedWebElements = Enumerable.Empty<ILocator>();
			try
			{
				Log.Verbose($"Try get elements by {locatorName} with path '{locatorValue}' for timeout '{timeout}'");
				searchedWebElements = await FindElements(locatorName, locatorValue, timeout, searchContext);
				Log.Verbose(
					"Elements {by} has been found, count: {count}.",
					$"{locatorName} : {locatorValue}",
					searchedWebElements.Count());
			}
			catch (Exception ex)
			{
				Log.Error(
					"Exception occurred while searching for elements by {by}: {exception}.",
					$"{locatorName} : {locatorValue}",
					ex.Message);
			}

			var elements = searchedWebElements
				.Select(searchedWebElement =>
				{
					var elementInstance = Activator.CreateInstance(typeof(T)) as IWebBlockInternal;
					elementInstance.InitBlock(
						this,
						PlaywrightWebElementImplementation.GetNewInstance(searchedWebElement),
						locatorName,
						locatorValue,
						null);
					return (T)elementInstance;
				});

			return elements;
		}

		private async Task<ILocator> FindElement(
			By locatorName,
			string locatorValue,
			TimeSpan? timeout = null,
			IWebBlock elementContext = null)
		{
			ILocator searchContext = elementContext is null
				? Page.Locator(locatorValue)
				: (elementContext as IWebBlockInternal).WebElement as ILocator;

			var timeoutValue = (float)(timeout?.TotalMilliseconds ?? ElementWaitingTimeout.TotalMilliseconds);
			Log.Verbose(
				"Search for element by {by} for {timeout} seconds.",
				$"{locatorName} : {locatorValue}",
				timeoutValue);

			await searchContext.WaitForAsync(new LocatorWaitForOptions { Timeout = timeoutValue,  });
			return searchContext;
		}

		private async Task<IEnumerable<ILocator>> FindElements(
			By locatorName,
			string locatorValue,
			TimeSpan? timeout = null,
			IWebBlock elementContext = null)
		{
			ILocator searchContext = elementContext is null
				? Page.Locator(locatorValue)
				: ((elementContext as IWebBlockInternal).WebElement as ILocator).Locator(locatorValue);

			var timeoutValue = (float)(timeout?.TotalMilliseconds ?? ElementWaitingTimeout.TotalMilliseconds);
			Log.Verbose(
				"Search for elements by {by} for {timeout} seconds.",
				$"{locatorName} : {locatorValue}",
				timeoutValue);

			var resultsCount = await searchContext.CountAsync();

			if (resultsCount == 0)
			{
				return Enumerable.Empty<ILocator>();
			}
			
			var resulElements = await searchContext.AllAsync();
			return resulElements;
		}

		private string GetFilePath(string directory, string fileName)
		{
			var newFileName = fileName
				.Replace("\"", string.Empty)
				.Replace("\\", string.Empty)
				.Replace("(", "_")
				.Replace(")", "_");
			return Path.Combine(directory, newFileName + ".png");
		}

		public async Task<T> ExecuteJavaScriptAsync<T>(string script, params object[] args)
		{
			return await Page.EvaluateAsync<T>(script, args);
		}

		internal IBrowser Browser { get; set; }

		internal IInternalPage Page { get; set; }

		internal IPlaywright Playwright { get; set; }


		private static readonly TimeSpan ElementWaitingTimeout = TimeSpan.FromSeconds(10);
	}
}