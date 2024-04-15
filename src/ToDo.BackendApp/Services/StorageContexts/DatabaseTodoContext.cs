using Bogus;
using System.Collections.Generic;
using System.Linq;
using ToDo.BackendApp.Models;

namespace ToDo.BackendApp.Services.StorageContexts
{
	public class DatabaseTodoContext : IStorageContext<Todo>
	{
		public DatabaseTodoContext()
		{
			var faker = new Faker<Todo>()
				.StrictMode(true)
				.RuleFor(p => p.Id, f => f.Random.Number(1, 1000))
				.RuleFor(p => p.Title, f => f.Hacker.Adjective())
				.RuleFor(p => p.Description, f => f.Hacker.Phrase())
				.RuleFor(p => p.IsDone, f => f.Random.Bool());

			_todos = Enumerable.Range(1, 5).Select(_ => faker.Generate()).ToList();
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