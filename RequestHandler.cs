using System.Net;
using System.Text;
using System.Text.Json;

namespace InterServer;

public class RequestHandler
{
	public RequestHandler(HttpListenerRequest request, HttpListenerResponse response, bool dryRun = true) //
	{
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

		
		TestJsonData ?postObj = JsonSerializer.Deserialize<TestJsonData>(postData);

		if (postObj != null)
		{
			Console.WriteLine($"And the same thing, but obj:\n{postObj.key1}\n{postObj.key2}");
		}
		
		
		// A thing for handling response text
		ResponseManager(response, ResponseType.Ok);
	}

	//TODO: implement this
	private void ResponseManager(HttpListenerResponse response, ResponseType responseType, bool reject = false)
	{
		string responseStr;
		
		
		
		if (reject)
		{
			// TODO: make actual reject MSG
			responseStr = "Rejected.";
		}
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
		responseStr = sillyCat;
		
		byte[] responseBytes = Encoding.UTF8.GetBytes(responseStr);
		
		
		response.ContentLength64 = responseBytes.Length;
		response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
		response.Close();
	}
	

	private string GetJson()
	{
		return "NIY";
	}
}