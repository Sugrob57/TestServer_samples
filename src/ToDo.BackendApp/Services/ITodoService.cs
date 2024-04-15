using System.Collections.Generic;
using System.Threading.Tasks;
using ToDo.BackendApp.Models;

namespace ToDo.BackendApp.Services
{
	public interface ITodoService
	{
		public Task<Todo?> FindAsync(int id);

		public Task<List<Todo>> GetAllAsync();

		public Task AddAsync(Todo todo);

		public Task UpdateAsync(Todo todo);

		public Task<bool> RemoveAsync(int id);

		public Task<List<Todo>> GetIncompleteTodosAsync();
	}
}