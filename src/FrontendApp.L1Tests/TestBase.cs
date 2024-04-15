using QaTools.WebTests.Core.Abstractions;
using TestHostLibrary.Services;

namespace FrontendApp.L1Tests
{
	public abstract class TestBase
	{
		[TearDown]
		public void BaseTearDown()
		{
			WebBrowser.Dispose();
		}

		protected IWebBrowser WebBrowser
		{
			get => NUnitExtensions.GetTestContextProperty<IWebBrowser>(nameof(IWebBrowser));
			set => NUnitExtensions.SetTestContextProperty(nameof(IWebBrowser), value);
		}
	}
}