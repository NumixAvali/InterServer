﻿using InterServer.Logic;
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
		var data = new DbHandler(
			"192.168.2.116",
			"Measurements",
			"dbadmin",
			""
			// ).GetDataByTimestamp(1709216936);
			// ).GetLatestData();
		).GetDataRange(1709216809, 1709216936);

		// var data = new RequestHandler().GetJson();

		return Ok(data);
	}

	[MapToApiVersion("1.0")]
	[HttpGet, Route("get-latest-cache")]
	[DisableRateLimiting]
	public ReplyJson GetLatestCachedData()
	{
		return new RequestHandler().ResponseManager(ResponseType.Ok, ReplyDataType.CachedLatestData);
	}

	[MapToApiVersion("1.0")]
	[HttpPost, Route("get-cache")]
	// [DisableRateLimiting]
	// public ReplyJson GetCachedData()
	public ActionResult<ReplyJson> GetCachedData([FromBody] RequestJson postData)
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
	[HttpGet, Route("get-data")]
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
}

