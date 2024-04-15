using ToDo.BackendApp.Models;

namespace ToDo.BackendApp.Services
{
	public class DataStorage
	{
		public DataStorage(IStorageContext<Todo> todoContext)
		{
			Todos = todoContext;
		}

		public IStorageContext<Todo> Todos { get; private set; }
	}
}