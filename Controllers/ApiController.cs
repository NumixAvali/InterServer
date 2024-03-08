using InterServer.Logic;
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
		AppSettings appSettings = new SettingsController().GetSettings();
		
		var data = new DbHandler(
			appSettings.DbIp,
			appSettings.DbName,
			appSettings.DbUsername,
			appSettings.DbPassword
			// ).GetDataByTimestamp(1709216936);
			// ).GetLatestData();
		).GetDataRange(1709216809, 1709216936);

		// var data = new RequestHandler().GetJson();

		return Ok(data.Data);
	}

	[MapToApiVersion("1.0")]
	[HttpGet, Route("latest-cache")]
	[DisableRateLimiting]
	public ReplyJsonNested GetLatestCachedData()
	{
		
		try
		{
			return new RequestHandler().ResponseManager(ResponseType.Ok, ReplyDataType.CachedLatestData);
		}
		catch
		{
			return new RequestHandler().ResponseManager(ResponseType.UnknownError, ReplyDataType.NoData);
		}

	}

	[MapToApiVersion("1.0")]
	[HttpPost, Route("historic-cache")]
	// [DisableRateLimiting]
	// public ReplyJson GetCachedData()
	public ActionResult<ReplyJsonNested> GetCachedData([FromBody] RequestJson postData)
	{
		if (String.IsNullOrEmpty(postData.Token) ) return new RequestHandler().ResponseManager(ResponseType.AuthReject);
		if (postData.Timestamp == 0) return new RequestHandler().ResponseManager(ResponseType.IncorrectJson);

		DataJson additionalData = new DataJson
		{
			Status = ResponseType.Ok,
			Data = postData.Timestamp.ToString()
		};

		return new RequestHandler().ResponseManager(ResponseType.Ok, ReplyDataType.CachedPeriodData, additionalData);
	}

	[MapToApiVersion("1.0")]
	[HttpGet, Route("current-data")]
	public ReplyJson GetCurrentData()
	{
		try
		{
			return new RequestHandler().ResponseManager(ResponseType.Ok, ReplyDataType.CurrentData);
		}
		catch
		{
			return new RequestHandler().ResponseManager(ResponseType.UnknownError, ReplyDataType.NoData);
		}
		
	}
	
	[MapToApiVersion("1.0")]
	[HttpPost, Route("historic-cache-range")]
	// [DisableRateLimiting]
	public ActionResult<ReplyJsonList> GetCachedDataRange([FromBody] RequestJsonRange postData)
	{
		if (String.IsNullOrEmpty(postData.Token) ) return new RequestHandler().ResponseManager(ResponseType.AuthReject);
		if (postData.TimestampStart == 0) return new RequestHandler().ResponseManager(ResponseType.IncorrectJson);
		if (postData.TimestampEnd == 0) return new RequestHandler().ResponseManager(ResponseType.IncorrectJson);

		uint[] timestampArray = new[] { postData.TimestampStart, postData.TimestampEnd };
		
		DataJson additionalData = new DataJson
		{
			Status = ResponseType.Ok,
			Data = timestampArray
		};

		return new RequestHandler().ResponseManager(ResponseType.Ok, ReplyDataType.CachedRangeData, additionalData);
	}

}

