using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InterServer.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}")] // Add "/[controller]" in case of transition to routed endpoints (https://medium.com/@dipendupaul/api-versioning-in-net-7-6db4fa9d2d99)
[ApiVersion("1.0")]
[EnableRateLimiting("fixed")]
public class ApiController : ControllerBase
{
	[MapToApiVersion("1.0")]
	[HttpGet, Route("test")]
	public IActionResult Test()
	{
		var data = new { message = "Hello, API!" };

		// var data = new RequestHandler().GetJson();

		return Ok(data);
	}

	[MapToApiVersion("1.0")]
	[HttpGet, Route("get-cache")]
	[DisableRateLimiting]
	public ReplyJson GetCachedData()
	{
		// var reply = new RequestHandler().ResponseManager(ResponseType.Ok, ReplyDataType.CachedData);
		
		return new RequestHandler().ResponseManager(ResponseType.Ok, ReplyDataType.CachedData);
	}
	
	[MapToApiVersion("1.0")]
	[HttpGet, Route("get-data")]
	public ReplyJson GetCurrentData()
	{

		// var reply = new RequestHandler().ResponseManager(ResponseType.Ok);
		
		// TODO: make ResponseType dynamic, and not hardcoded to "Ok"
		try
		{
			return new RequestHandler().ResponseManager(ResponseType.Ok, ReplyDataType.CurrentData);
		}
		catch
		{
			return new RequestHandler().ResponseManager(ResponseType.UnknownError, ReplyDataType.NoData);
		}
		
	}
}

