using ToDo.BackendApp.Models;

namespace Testing.Common.Helpers
{
	public class FakeDataGenerators
	{
		public static List<Todo> GetTestTodos(int count)
		{
			var todos = new List<Todo>();

			for (int i = 0; i < count; i++)
			{
				todos.Add(new Todo
				{
					Id = i,
					Title = $"{i}_{Guid.NewGuid()}",
					Description = Guid.NewGuid().ToString(),
					IsDone = i % 2 == 0
				});
			}

			return todos;
		}
	}
}