using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ToDo.BackendApp.Services
{
	public class RemoteSmtpEmailService : IEmailService
	{
		public RemoteSmtpEmailService(ApplicationSettings applicationSettings)
		{
			SmtpServerAddress = applicationSettings.GetSetting("SmtpServerAddress");
		}

		public async Task SendAsync(string email, string message)
		{
			using var httpClient = new HttpClient();
			var response = await httpClient.PostAsJsonAsync(
			$"{SmtpServerAddress}/api/email",
			new
			{
				To = email,
				Message = message
			});

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"remote SMTP server error: {response.Content}");
			}
		}

		private string SmtpServerAddress { get; set; }
	}
}