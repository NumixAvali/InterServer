namespace InterServer;

public enum JsonRequestType : ushort
{
	Get = 0,
	Test = 1
}
// This fucking sucks, I hate it!
//TODO: come up with something else
public enum ResponseType : ushort
{
	IncorrectJson = 0,
	AuthReject = 1,
	ServiceUnavailable = 2,
	UnknownError = 3,
	Ok = 4,
	Rejected = 5
}


public class GetJson
{
	public bool? ForceRefresh { get; set; }
	public string? Token { get; set; }
	public ushort? RequestType { get; set; } 
}

public class TestJsonData
{
	public string? key1 { get; set; }
	public string? key2 { get; set; }
}
