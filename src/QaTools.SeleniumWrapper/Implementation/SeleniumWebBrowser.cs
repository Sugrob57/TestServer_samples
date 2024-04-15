using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using QaTools.WebTests.Core.Abstractions;
using QaTools.WebTests.Core.Helpers;
using Serilog;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using static QaTools.SeleniumWrapper.Implementation.SeleniumLocatorConverter;
using By = QaTools.WebTests.Core.Abstractions.By;

namespace QaTools.SeleniumWrapper.Implementation
{
	public class SeleniumWebBrowser : IWebBrowser, IDisposable
	{
		public SeleniumWebBrowser()
		{
			if (WebDriver == null)
			{
				Log.Debug("Start initialize chrome browser");
				WebDriver = BrowserFactory.GetWebDriver();
			}
		}

		public async Task<TPage> GetPage<TPage>()
			where TPage : class, IPage
		{
			await AsyncActions
				.WaitForExternalEventAsync(async () =>
				{
					var state = await ExecuteJavaScriptAsync<string>("return document.readyState");
					return state.Equals("complete");
				});

			var page = (TPage)Activator.CreateInstance(typeof(TPage)) as IPageInternal;
			var webPage = (WebPage)Activator.CreateInstance(page.WebPageType, this);
			page.SetWebPage(webPage);

			return page as TPage;
		}

		public string Url => WebDriver.Url;

		public string Title => WebDriver.Title;

		public async Task RefreshAsync()
		{
			Log.Debug("Refresh page.");
			WebDriver.Navigate().Refresh();
			await Task.CompletedTask;
			try
			{
				var alert = WebDriver.SwitchTo().Alert();
				alert.Accept();
			}
			catch (NoAlertPresentException)
			{
			}
		}

		public async Task BackAsync()
		{
			Log.Debug("Back page.");
			WebDriver.Navigate().Back();
			Log.Debug($"CurrentUrl: {WebDriver.Url}");
			await Task.CompletedTask;
		}

		public async Task GoToUrlAsync(string url)
		{
			try
			{
				WebDriver.Navigate().GoToUrl(url);
				await Task.CompletedTask;
			}
			catch (WebDriverTimeoutException)
			{
				Log.Error("Timeout occured while going to url {url}.", url);
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

			var screenshot = WebDriver.TakeScreenshot();
			screenshot.SaveAsFile(filePath);
			await Task.CompletedTask;
			return filePath;
		}

		public async Task MaximizeWindowAsync()
		{
			WebDriver.Manage().Window.Maximize();
			await Task.CompletedTask;
		}

		public async Task<T> ExecuteJavaScriptAsync<T>(string script, params object[] args)
		{
			await Task.CompletedTask;
			return WebDriver.ExecuteJavaScript<T>(script, args);
		}

		public void Dispose()
		{
			if (WebDriver != null)
			{
				Log.Debug("Quit from WebBrowser");

				WebDriver?.Close();
				WebDriver?.Quit();
				WebDriver = null;

				Log.Debug("Quit from WebBrowser were successful");
				WebDriver = null;
			}
		}

		public async Task<IWebBlock> GetElementBy(
			By locatorName, string locatorValue, TimeSpan? timeout = null)
		{
			await Task.CompletedTask;
			return InitWebBlock(() => FindElement(locatorName, locatorValue, timeout), locatorName, locatorValue);
		}

		public async Task<T> GetElementBy<T>(
			By locatorName, string locatorValue, TimeSpan? timeout = null)
			where T : class, IWebBlock
		{
			await Task.CompletedTask;
			return InitWebBlock<T>(() => FindElement(locatorName, locatorValue, timeout), locatorName, locatorValue);
		}

		public async Task<IEnumerable<IWebBlock>> GetElementsBy(
			By locatorName, string locatorValue, TimeSpan? timeout = null)
		{
			return await GetElementsBy<WebBlock>(locatorName, locatorValue, timeout);
		}

		public async Task<IEnumerable<T>> GetElementsBy<T>(
			By locatorName, string locatorValue, TimeSpan? timeout = null, IWebBlock searchContext = null)
			where T : class, IWebBlock
		{
			await Task.CompletedTask;
			var searchedWebElements = Enumerable.Empty<IWebElement>();
			try
			{
				Log.Verbose($"Try get elements by {locatorName} with path '{locatorValue}' for timeout '{timeout}'");
				searchedWebElements = FindElements(locatorName, locatorValue, timeout, searchContext);
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
						SeleniumWebElementImplementation.GetNewInstance(searchedWebElement),
						locatorName,
						locatorValue,
						null);
					return (T)elementInstance;
				});

			return elements;
		}

		public async Task<IWebBlock> GetElementFromElementBy(
			IWebBlock anotherElement, By locatorName, string locatorValue, TimeSpan? timeout = null) =>
			InitWebBlock(
				() => FindElement(locatorName, locatorValue, timeout, anotherElement),
				locatorName,
				locatorValue);

		public async Task<T> GetElementFromElementBy<T>(
			IWebBlock anotherElement, By locatorName, string locatorValue, TimeSpan? timeout = null)
			where T : class, IWebBlock =>
			InitWebBlock<T>(
				() => FindElement(locatorName, locatorValue, timeout, anotherElement), locatorName, locatorValue);

		public async Task<IEnumerable<T>> GetElementsFromElementBy<T>(
			IWebBlock anotherElement, By locatorName, string locatorValue, TimeSpan? waitTimeout = null)
			where T : class, IWebBlock =>
			await GetElementsBy<T>(locatorName, locatorValue, waitTimeout, anotherElement);

		private WebBlock InitWebBlock(
			Func<IWebElement> action, By locatorName, string locatorValue, WebBlock parentElement = null) =>
			InitWebBlock<WebBlock>(action, locatorName, locatorValue, parentElement);

		private T InitWebBlock<T>(
			Func<IWebElement> action, By locatorName, string locatorValue, WebBlock parentElement = null)
			where T : class, IWebBlock
		{
			IWebElement element = null;
			var staleElement = true;
			Log.Verbose($"Wait element by {locatorName} with path '{locatorValue}'");

			while (staleElement)
			{
				try
				{
					element = action();
					staleElement = false;
				}
				catch (StaleElementReferenceException)
				{
					Log.Error(
						"StaleElementReferenceException occurred while waiting for element by {by}: {exception}.",
						$"{locatorName} : {locatorValue}");
					staleElement = true;
				}
				catch (Exception ex)
				{
					Log.Error(
						"Exception occurred while waiting for element by {by}: {exception}.",
						$"{locatorName} : {locatorValue}",
						ex.Message);
					break;
				}
			}

			Log.Verbose($"Found element by {locatorName} with path '{locatorValue}'");
			var elementInstance = Activator.CreateInstance(typeof(T)) as IWebBlockInternal;

			elementInstance.InitBlock(
				this,
				SeleniumWebElementImplementation.GetNewInstance(element),
				locatorName,
				locatorValue,
				parentElement);

			return (T)elementInstance;
		}

		private IWebElement FindElement(
			By locatorName,
			string locatorValue,
			TimeSpan? timeout = null,
			IWebBlock elementContext = null)
		{
			var searchContext = elementContext is null
				? WebDriver
				: (elementContext as IWebBlockInternal).WebElement as ISearchContext;

			timeout = timeout ?? ElementWaitingTimeout;
			Log.Verbose(
				"Search for element by {by} for {timeout} seconds.",
				$"{locatorName} : {locatorValue}",
				timeout.Value);

			var wait = new WebDriverWait(WebDriver, timeout.Value);
			wait.Until(d => searchContext.FindElements(ToSeleniumBy(locatorName, locatorValue)).Any());
			return searchContext.FindElement(SeleniumLocatorConverter.ToSeleniumBy(locatorName, locatorValue));
		}

		private IEnumerable<IWebElement> FindElements(
			By locatorName,
			string locatorValue,
			TimeSpan? timeout = null,
			IWebBlock elementContext = null)
		{
			var searchContext = elementContext is null
				? WebDriver
				: (elementContext as IWebBlockInternal).WebElement as ISearchContext;

			timeout = timeout ?? ElementWaitingTimeout;
			Log.Verbose(
				"Search for elements by {by} for {timeout} seconds.",
				$"{locatorName} : {locatorValue}",
				timeout.Value);

			var resulElements = Enumerable.Empty<IWebElement>();
			var wait = new WebDriverWait(WebDriver, timeout.Value);
			wait.Until(d =>
			{
				resulElements = searchContext.FindElements(ToSeleniumBy(locatorName, locatorValue));
				return resulElements.Any();
			});

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

		internal IWebDriver WebDriver { get; set; }

		private static readonly TimeSpan ElementWaitingTimeout = TimeSpan.FromSeconds(10);
	}
}