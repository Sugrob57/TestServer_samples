using BackendApi.L1Tests.Tests;
using Serilog;
using ToDo.BackendApp.Models;
using ToDo.BackendApp.Services;

namespace BackendApi.L1Tests.Fixtures
{
	public class ToDoServiceProxy : ITodoService
	{
		public ToDoServiceProxy(TodoService todoService)
		{
			_todoService = todoService;
		}

		public Task<Todo?> FindAsync(int id)
		{
			return _todoService.FindAsync(id);
		}

		public Task<List<Todo>> GetAllAsync()
		{
			return _todoService.GetAllAsync();
		}

		public Task AddAsync(Todo todo)
		{
			return Task.CompletedTask;
		}

		public async Task UpdateAsync(Todo todo)
		{
			if (_interceptActions.TryGetValue(todo.Id, out var action))
			{
				Log.Debug("Intercept UpdateAsync for Id {id}", todo.Id);
				action.Invoke();
			}
			else
			{
				await _todoService.UpdateAsync(todo);
			}
		}

		public Task<bool> RemoveAsync(int id)
		{
			return _todoService.RemoveAsync(id);
		}

		public Task<List<Todo>> GetIncompleteTodosAsync()
		{
			return _todoService.GetIncompleteTodosAsync();
		}

		private Dictionary<int, Action> _interceptActions => UseProxyServiceTests.TodoServiceInterceptActions;

		private readonly ITodoService _todoService;
	}
}
