using NUnit.Framework.Internal;

namespace TestHostLibrary.Services
{
	public class NUnitExtensions
	{
		public static void SetTestContextProperty(string key, object value)
		{
			TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(key, value);
		}

		public static T? GetTestContextProperty<T>(string key)
		{
			if (TestExecutionContext.CurrentContext.CurrentTest.Properties.ContainsKey(key))
			{
				return (T)TestExecutionContext.CurrentContext.CurrentTest.Properties.Get(key);
			}

			return default;
		}
	}
}