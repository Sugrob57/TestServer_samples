namespace QaTools.WebTests.Core.Abstractions
{
	/// <summary>
	/// Abstract public Web Block entity, representing block element on page or as a part of a complex block.
	/// </summary>
	public interface IWebBlock
	{
		string FullLocator { get; }

		string Text { get; }

		bool Exists { get; }

		bool Enabled { get; }

		bool Selected { get; }

		bool Displayed { get; }

		void Clear();

		void SendKeys(string text);

		void Click();
	}
}