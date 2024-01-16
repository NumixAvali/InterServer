using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace InterServer;

public class RequestHandler
{
	// public RequestHandler(HttpListenerRequest request, HttpListenerResponse response, bool dryRun = true) //
	// {
	// 	// Stop the logic, in case of artificial summon
	// 	if (request == null || response == null) return;
	//
	// 	// Private method picker
	// 	switch (request.HttpMethod)
	// 	{
	// 		case "POST":
	// 		{
	// 			PostHandler(request, response);
	// 			break;
	// 		}
	// 		case "GET":
	// 		{
	// 			GetHandler(response);
	// 			break;
	// 		}
	// 	}
	// 	
	// }

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

	public ReplyJson ResponseManager(ResponseType responseType, ReplyDataType dataType = ReplyDataType.NoData)
	{
		// TODO: passed arguments are must be a suggestion at this point.
		// Internal logic should be able to determine final answer itself. 
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
		
		string responseStr = "Internal error!";

		string dataResponse = sillyCat;

		Dictionary<ResponseType, string> responseMessages = new Dictionary<ResponseType, string>
		{
			{ ResponseType.Ok, "Ok." },
			{ ResponseType.AuthReject, "Authorization error." },
			{ ResponseType.IncorrectJson, "Malformed JSON provided." },
			{ ResponseType.ServiceUnavailable, "Service temporary unavailable. Try again later." },
			{ ResponseType.UnknownError, "Unknown server error. Try again later." },
			{ ResponseType.Rejected, "Rejected." },
		};
		
		Dictionary<ReplyDataType, string> responseDataType = new Dictionary<ReplyDataType, string>
		{
			{ ReplyDataType.CachedData, GetCachedJson() },
			{ ReplyDataType.CurrentData, GetJson() },
			{ ReplyDataType.NoData, sillyCat },
		};
		
		if (responseMessages.TryGetValue(responseType, out var message))
		{
			responseStr = message;
		}
		
		if (responseDataType.TryGetValue(dataType, out var dataMessage))
		{
			dataResponse = dataMessage;
		}


		var reply = new ReplyJson
		{
			message = responseStr,
			data = dataResponse
		};

		
		return reply;
	}
	

	private string GetJson()
	{
		const bool verbose = false;
		string sillyCat =
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

		const string inverterIp = "192.168.2.211";
		const int inverterPort = 8899;

		DataProcessor dataProcessor = new DataProcessor();

		byte[] frame = dataProcessor.ConstructFrame();
		FrameInfo digestedInfo = new FrameInfo();
		

		// if (verbose)
		// {
		// 	Console.WriteLine("Assembled frame:");
		// 	Console.WriteLine(BitConverter.ToString(frame).Replace("-", " "));
		// 	// Console.WriteLine(Convert.ToBase64String(frame));
		//
		// 	Console.WriteLine("Based string converted:");
		// 	Console.WriteLine(BitConverter.ToString(unbased1).Replace("-", " "));
		// 	// Console.WriteLine(Convert.ToBase64String(unbased1));
		// }
		
		
		//TCP client
		
		using (TcpClient tcpClient = new TcpClient())
		{
			try
			{
				tcpClient.Connect(inverterIp, inverterPort);

				if (verbose) Console.WriteLine("Connected to the server.");
				int responses = 0;
				while (responses < 2)
				{
					// Send data to the server
					tcpClient.GetStream().Write(dataProcessor.ConstructFrame(responses), 0, dataProcessor.ConstructFrame(responses).Length);

					if (verbose) Console.WriteLine($"Message #{responses} was sent to the inverter");

					// Receive data from the server
					byte[] buffer = new byte[1024];
					int bytesRead = tcpClient.GetStream().Read(buffer, 0, buffer.Length);

					if (verbose)
					{
						Console.WriteLine("Reply from the inverter, length: " + bytesRead + " bytes");
						Console.WriteLine(Convert.ToBase64String(buffer));
					}

					digestedInfo = dataProcessor.DigestResponse(buffer);

					responses++;
				}

				return JsonSerializer.Serialize(digestedInfo) ?? sillyCat; // BitConverter.ToString(buffer).Replace("-", " ");
			}
			catch (Exception ex)
			{
				Console.WriteLine("An error occurred: " + ex.Message);
			}
		}

		// This point should never be reached.
		// But if somehow it will be, someone will get an easter egg!
		return sillyCat;
	}

	private string GetCachedJson()
	{
		string otherSillyCat =
@"⠀⠀⣼⠲⢤⡀⣀⣐⣷⢤⡀⠀⠀⠀⠀⢀⣀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⡇⠀⠀⠘⠷⣀⠀⠀⠈⢻⢤⠶⠛⠉⢸⡇⠀⠀⠀⠀⠀⠀⠀
⠀⠸⡁⠀⢀⣠⣄⠀⠀⠀⡀⣀⠀⠀⠀⠀⡾⠁⠀⠀⠀⠀⠀⠀⠀
⢀⡀⣇⢰⡟⢰⣿⠀⠀⣼⡇⠈⠳⡄⠀⣰⠃⠀⠀⠀⠀⠀⠀⠀⠀
⠙⢧⡀⠸⠀⢘⣛⡀⠀⠻⠇⠀⣰⠃⢚⣳⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠸⠵⠢⣄⣀⠈⣠⡀⠀⠀⠀⢉⣠⣿⡁⠀⠀⠀⠀⠀⠀⠀⡀⠀
⠀⠀⠀⠀⢲⣟⠛⠀⠀⠀⠉⠉⣿⠃⠀⠀⠀⠀⠀⠀⠀⢀⠀⡏⢳
⠀⠀⠀⠀⠠⢯⡀⠀⠀⠀⠀⠀⢼⠛⠀⠀⠀⠀⠀⠀⠀⢸⡦⠃⠘
⠀⠀⠀⠀⠀⢸⡇⠀⠘⣾⠇⠀⠸⣦⠀⠀⠀⠀⠀⠀⠀⣰⠀⠀⠀
⠀⠀⠀⠀⠀⠀⣶⠀⠀⢻⡆⠀⢸⡇⠉⠲⣄⠀⣀⡀⡶⠃⠀⠀⠀
⠀⠀⠀⠀⠀⠀⢿⠀⠀⠀⠀⠀⢸⡇⠀⠀⢹⡏⠁⠀⠀⠀⠀⠀⠀";

		return otherSillyCat;
	}
	

}