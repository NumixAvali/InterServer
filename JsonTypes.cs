namespace InterServer;

public enum JsonRequestType : ushort
{
	Get = 0,
	Test = 1
}

public enum ResponseType : ushort
{
	IncorrectJson = 0,
	AuthReject = 1,
	ServiceUnavailable = 2,
	UnknownError = 3,
	Ok = 4,
	Rejected = 5
}

public enum ReplyDataType : ushort
{
	CurrentData = 0,
	CachedData = 1,
	NoData = 2
}


public class RequestJson
{
	public bool? ForceRefresh { get; set; }
	public string? Token { get; set; }
	public ResponseType RequestType { get; set; } 
}

public class ReplyJson
{
	public string message { get; set; }
	public string? data { get; set; }
}

public class TestJsonData
{
	public string? key1 { get; set; }
	public string? key2 { get; set; }
}
