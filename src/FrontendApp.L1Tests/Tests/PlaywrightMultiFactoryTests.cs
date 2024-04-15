using FluentAssertions;
using QaTools.WebTests.FrontendAppPages.PageObjects;
using TestHostLibrary.Services;
using FrontendAppProgram = ToDo.FrontendApp.Program;
using BackendAppProgram = ToDo.BackendApp.Program;
using QaTools.PlaywrightWrapper;

namespace FrontendApp.L1Tests.Tests
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class PlaywrightMultiFactoryTests : TestBase
	{
		[Test]
		public async Task MultiAppFactory_RunTwoApplications_DataLoaded()
		{
			// arrange
			MultiAppFactory = new MultiAppFactory<FrontendAppProgram, BackendAppProgram>();

			// act
			var endpoint = $"{MultiAppFactory.FrontendAddress}Home";

			WebBrowser = await new PlaywrightChromeWebBrowser().InitAsync();
			await WebBrowser.GoToUrlAsync(endpoint);

			var page = await WebBrowser.GetPage<IndexPage>();
			var title = page.TableTitle;
			var todoList = page.TableRecords;

			// assert
			WebBrowser.Url.Should().StartWith($"https://localhost:").And.EndWith("/Home");
			title.Should().Contain("5");
			todoList.Count().Should().Be(5);
		}

		[TearDown]
		public void ClassTearDown()
		{
			MultiAppFactory.Dispose();
		}

		private MultiAppFactory<FrontendAppProgram, BackendAppProgram> MultiAppFactory
		{
			get => NUnitExtensions.GetTestContextProperty<MultiAppFactory<FrontendAppProgram, BackendAppProgram>>(nameof(MultiAppFactory));
			set => NUnitExtensions.SetTestContextProperty(nameof(MultiAppFactory), value);
		}
	}
}