using System.Net;
using System.Text;
using System.Text.Json;

namespace InterServer;

public class RequestHandler
{
	public RequestHandler(HttpListenerRequest request, HttpListenerResponse response, bool dryRun = true) //
	{
		// Stop the logic, in case of artificial summon
		if (request == null || response == null) return;

		// Private method picker
		switch (request.HttpMethod)
		{
			case "POST":
			{
				PostHandler(request, response);
				break;
			}
			case "GET":
			{
				GetHandler(response);
				break;
			}
		}
		
	}

	private void GetHandler(HttpListenerResponse response)
	{
		const string placeHolderRespose = "TODO: Make actual website for displaying data, etc.";
		
		byte[] responseBytes = Encoding.UTF8.GetBytes(placeHolderRespose);
		
		
		response.ContentLength64 = responseBytes.Length;
		response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
		response.Close();
	}

	private void PostHandler(HttpListenerRequest request, HttpListenerResponse response)
	{
		Stream body = request.InputStream;
		Encoding encoding = request.ContentEncoding;
		StreamReader reader = new StreamReader(body, encoding);

		string postData = reader.ReadToEnd();
		
		Console.WriteLine($"POST request data:\n{postData}");
		body.Close();
		reader.Close();

		RequestJson incomingJson = JsonSerializer.Deserialize<RequestJson>(postData)!;
		// TestJsonData ?postObj = JsonSerializer.Deserialize<TestJsonData>(postData);
		//
		// if (postObj != null)
		// {
		// 	Console.WriteLine($"And the same thing, but obj:\n{postObj.key1}\n{postObj.key2}");
		// }
		
		
		// A thing for handling response text
		ResponseManager(incomingJson.RequestType);
	}

	private void ResponseManager(ResponseType responseType)
	{
		string responseStr = "Internal error!";
		
		const string sillyCat =
			@"⣿⣿⡟⡹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
⣿⣿⢱⣶⣭⡻⢿⠿⣛⣛⣛⠸⣮⡻⣿⣿⡿⢛⣭⣶⣆⢿⣿
⣿⡿⣸⣿⣿⣿⣷⣮⣭⣛⣿⣿⣿⣿⣶⣥⣾⣿⣿⣿⡷⣽⣿
⣿⡏⣾⣿⣿⡿⠿⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⣿⣿
⣿⣧⢻⣿⡟⣰⡿⠁⢹⣿⣿⣿⣋⣴⠖⢶⣝⢻⣿⣿⡇⣿⣿
⠩⣥⣿⣿⣴⣿⣇⠀⣸⣿⣿⣿⣿⣷⠀⢰⣿⠇⣿⣭⣼⠍⣿
⣿⡖⣽⣿⣿⣿⣿⣿⣿⣯⣭⣭⣿⣿⣷⣿⣿⣿⣿⣿⡔⣾⣿
⣿⡡⢟⡛⠻⠿⣿⣿⣿⣝⣨⣝⣡⣿⣿⡿⠿⠿⢟⣛⣫⣼⣿
⣿⣿⣿⡷⠝⢿⣾⣿⣿⣿⣿⣿⣿⣿⣿⣾⡩⣼⣿⣿⣿⣿⣿
⣿⣿⣯⡔⢛⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣭⣍⣨⠿⢿⣿⣿⣿";
		
		// This probably can be moved to JsonTypes.cs
		Dictionary<ResponseType, string> responseMessages = new Dictionary<ResponseType, string>
		{
			{ ResponseType.Ok, sillyCat },
			{ ResponseType.AuthReject, "Authorization error." },
			{ ResponseType.IncorrectJson, "Malformed JSON provided." },
			{ ResponseType.ServiceUnavailable, "Service temporary unavailable. Try again later." },
			{ ResponseType.UnknownError, "Unknown server error. Try again later." },
			{ ResponseType.Rejected, "Rejected." },
		};

		if (responseMessages.TryGetValue(responseType, out var message))
		{
			responseStr = message;
		}


		// This solution below isn't good.
		ReplyJson reply;

		if (responseType == ResponseType.Ok)
		{
			reply = new ReplyJson
			{
				message = responseStr,
				data = GetJson()
			};
		}
		else
		{
			reply = new ReplyJson
			{
				message = responseStr,
				data = null
			};
		}
		
		

		
		byte[] responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(reply,new JsonSerializerOptions() { WriteIndented = true}) );
		
		
		// response.ContentLength64 = responseBytes.Length;
		// response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
		// response.Close();
	}
	

	public string GetJson()
	{
		const string sillyCat =
			@"⣿⣿⡟⡹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
⣿⣿⢱⣶⣭⡻⢿⠿⣛⣛⣛⠸⣮⡻⣿⣿⡿⢛⣭⣶⣆⢿⣿
⣿⡿⣸⣿⣿⣿⣷⣮⣭⣛⣿⣿⣿⣿⣶⣥⣾⣿⣿⣿⡷⣽⣿
⣿⡏⣾⣿⣿⡿⠿⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⣿⣿
⣿⣧⢻⣿⡟⣰⡿⠁⢹⣿⣿⣿⣋⣴⠖⢶⣝⢻⣿⣿⡇⣿⣿
⠩⣥⣿⣿⣴⣿⣇⠀⣸⣿⣿⣿⣿⣷⠀⢰⣿⠇⣿⣭⣼⠍⣿
⣿⡖⣽⣿⣿⣿⣿⣿⣿⣯⣭⣭⣿⣿⣷⣿⣿⣿⣿⣿⡔⣾⣿
⣿⡡⢟⡛⠻⠿⣿⣿⣿⣝⣨⣝⣡⣿⣿⡿⠿⠿⢟⣛⣫⣼⣿
⣿⣿⣿⡷⠝⢿⣾⣿⣿⣿⣿⣿⣿⣿⣿⣾⡩⣼⣿⣿⣿⣿⣿
⣿⣿⣯⡔⢛⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣭⣍⣨⠿⢿⣿⣿⣿";

		return sillyCat;
	}
}