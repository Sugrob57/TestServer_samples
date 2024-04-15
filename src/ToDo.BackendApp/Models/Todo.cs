using Newtonsoft.Json;

namespace ToDo.BackendApp.Models
{
	public class Todo
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; } = string.Empty;

		[JsonProperty("description")]
		public string Description { get; set; } = string.Empty;

		[JsonProperty("isDone")]
		public bool IsDone { get; set; }
	}
}