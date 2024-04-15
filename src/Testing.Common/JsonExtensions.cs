using Newtonsoft.Json;

namespace Testing.Common
{
	public static class JsonExtensions
	{
		public static string ToJson(this object value)
		{
			return JsonConvert.SerializeObject(value);
		}

		public static T FromJson<T>(this string value)
		{
			return JsonConvert.DeserializeObject<T>(value);
		}
	}
}