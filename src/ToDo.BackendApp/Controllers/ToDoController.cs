using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ToDo.BackendApp.Models;
using ToDo.BackendApp.Services;

namespace ToDo.BackendApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Produces("application/json")]
	public class ToDoController : Controller
	{
		private readonly ILogger<ToDoController> _logger;
		private readonly ITodoService _todoService;

		public ToDoController(
			ILogger<ToDoController> logger,
			ITodoService todoService)
		{
			_logger = logger;
			_todoService = todoService;
		}

		[HttpPost("new")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Create([FromBody] Todo item)
		{
			await _todoService.AddAsync(item);
			return CreatedAtAction(nameof(Create), new { id = item.Id }, item);
		}

		[HttpGet("records")]
		public async Task<IActionResult> GetTodos()
		{
			return Ok(await _todoService.GetAllAsync());
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] Todo item)
		{
			item.Id = id;
			await _todoService.UpdateAsync(item);
			return Ok(item);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (await _todoService.RemoveAsync(id))
			{
				NoContent();
			}

			return NotFound();
		}
	}
}