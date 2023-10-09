using Microsoft.AspNetCore.Mvc;

namespace InterServer.Controllers;

[Route("api")]
[ApiController]
public class ApiController : Controller
{
	[HttpGet("example")]
	public IActionResult GetExample()
	{
		var data = new { message = "Hello, API!" };

		return Ok(data);
	}
}