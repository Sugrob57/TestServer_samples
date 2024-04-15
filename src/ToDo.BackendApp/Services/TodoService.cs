using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDo.BackendApp.Models;

namespace ToDo.BackendApp.Services
{
	public class TodoService : ITodoService
	{
		public TodoService(DataStorage dbContext, ApplicationSettings applicationSettings, IEmailService emailService)
		{
			_dbContext = dbContext;
			_settings = applicationSettings;
			_emailService = emailService;
		}

		public async Task<Todo?> FindAsync(int id)
		{
			await Task.CompletedTask;
			return _dbContext.Todos.Find(id);
		}

		public async Task<List<Todo>> GetAllAsync()
		{
			await Task.CompletedTask;
			return _dbContext.Todos.ToList();
		}

		public async Task AddAsync(Todo todo)
		{
			await Task.CompletedTask;
			_dbContext.Todos.Add(todo);

			await _emailService.SendAsync(_settings.GetSetting("NotificationsEmail"), $"New todo has been added: {todo.Title}");
		}

		public async Task UpdateAsync(Todo todo)
		{
			await Task.CompletedTask;
			_dbContext.Todos.Update(todo);
		}

		public async Task<bool> RemoveAsync(int id)
		{
			await Task.CompletedTask;
			return _dbContext.Todos.Remove(id);
		}

		public async Task<List<Todo>> GetIncompleteTodosAsync()
		{
			await Task.CompletedTask;
			return _dbContext.Todos.ToList().Where(t => t.IsDone == false).ToList();
		}

		private readonly DataStorage _dbContext;
		private readonly ApplicationSettings _settings;
		private readonly IEmailService _emailService;
	}
}