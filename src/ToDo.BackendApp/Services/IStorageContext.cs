using System.Collections.Generic;

namespace ToDo.BackendApp.Services
{
	public interface IStorageContext<TItem>
	{
		public TItem Find(int id);

		public List<TItem> ToList();

		public void Add(TItem item);

		public void Update(TItem item);

		public bool Remove(int id);
	}
}