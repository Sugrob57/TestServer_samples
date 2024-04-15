using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Net;
using System.Text.RegularExpressions;

namespace ToDo.FrontendApp.Controllers
{
	public class PortalViewsController : Controller
	{
		private static readonly Regex PathValidationRegex = new Regex(@"^(?!.*\.{2,}.*)[a-zA-Z0-9/\.]+$", RegexOptions.Compiled);
	}
}