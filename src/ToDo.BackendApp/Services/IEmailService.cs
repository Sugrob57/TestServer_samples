using System.Threading.Tasks;

namespace ToDo.BackendApp.Services
{
	public interface IEmailService
	{
		public Task SendAsync(string email, string message);
	}
}