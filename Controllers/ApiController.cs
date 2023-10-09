using Microsoft.AspNetCore.Mvc;

namespace InterServer.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}")] // Add "/[controller]" in case of transition to routed endpoints (https://medium.com/@dipendupaul/api-versioning-in-net-7-6db4fa9d2d99)
[ApiVersion("1.0")]
public class ApiController : ControllerBase
{
	[MapToApiVersion("1.0")]
	[HttpGet("test")]
	public IActionResult Test()
	{
		// var data = new { message = "Hello, API!" };

		var data = new RequestHandler(null,null).GetJson();

		return Ok(data);
	}
}