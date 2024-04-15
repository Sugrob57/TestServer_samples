using Microsoft.AspNetCore.Mvc;

namespace ToDo.FrontendApp.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[Produces("application/json")]
	public class BackendAppController : ControllerBase
	{
		public BackendAppController(IConfiguration configuration)
		{
			_baseUrl = configuration.GetSection($"AppSettings:BackendUri").Value;
		}

		[Route("{*url}")]
		public IActionResult RedirectToApi(string url)
		{
			var fullUrl = $"{_baseUrl}api/{url}";
			return RedirectPreserveMethod(fullUrl);
		}

		private readonly string _baseUrl;
	}
}