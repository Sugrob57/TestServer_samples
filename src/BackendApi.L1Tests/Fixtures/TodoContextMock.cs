using ToDo.BackendApp.Models;
using ToDo.BackendApp.Services;

namespace BackendApi.L1Tests.Fixtures
{
	public class TodoContextMock : IStorageContext<Todo>
	{
		public TodoContextMock(List<Todo> todos)
		{
			_todos = todos;
		}

		public void Add(Todo item)
		{
			_todos.Add(item);
		}

		public Todo? Find(int id)
		{
			return _todos.FirstOrDefault(x => x.Id == id);
		}

		public bool Remove(int id)
		{
			var existent = Find(id);

			if (existent != null)
			{
				_todos.Remove(existent);
				return true;
			}

			return false;
		}

		public List<Todo> ToList()
		{
			return _todos;
		}

		public void Update(Todo item)
		{
			var existentTodo = _todos.First(x => x.Id == item.Id);
			existentTodo.Title = item.Title;
			existentTodo.Description = item.Description;
			existentTodo.IsDone = item.IsDone;
		}

		private List<Todo> _todos;
	}
}