using System.Net.Sockets;
using System.Text.Json;

namespace InterServer.Logic;

public class RequestHandler
{
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
		
		DataJson preFinalJson = new DataJson()
		{
			Status = ResponseType.UnknownError,
			Data = sillyCat,
		};
		
		var reply = new ReplyJson
		{
			Status = ResponseType.UnknownError,
			Message = "Internal error!",
			Data = null
		};


		Dictionary<ResponseType, string> responseMessages = new Dictionary<ResponseType, string>
		{
			{ ResponseType.Ok, "Ok." },
			{ ResponseType.AuthReject, "Authorization error." },
			{ ResponseType.IncorrectJson, "Malformed JSON provided." },
			{ ResponseType.ServiceUnavailable, "Service temporary unavailable. Try again later." },
			{ ResponseType.UnknownError, "Unknown server error. Try again later." },
			{ ResponseType.Rejected, "Rejected." },
			{ ResponseType.InternalError, "Internal Error."}
		};
		
		Dictionary<ReplyDataType, DataJson> responseDataType = new Dictionary<ReplyDataType, DataJson>
		{
			{ ReplyDataType.CachedData, GetCachedJson() },
			{ ReplyDataType.CurrentData, GetJson() },
			{ ReplyDataType.NoData, preFinalJson },
		};
		
		if (responseMessages.TryGetValue(responseType, out var message))
		{
			reply.Message = message;
		}
		
		if (responseDataType.TryGetValue(dataType, out var dataMessage))
		{
			preFinalJson = dataMessage;
		}

		switch (preFinalJson.Status)
		{
			case ResponseType.Ok:
			{
				reply.Status = ResponseType.Ok;
				reply.Message = responseMessages[ResponseType.Ok];
				reply.Data = JsonSerializer.Deserialize<FrameInfo>(preFinalJson.Data) ;
				break;
			}
			case ResponseType.InternalError:
			{
				reply.Status = ResponseType.InternalError;
				reply.Message = responseMessages[ResponseType.InternalError];
				reply.Data = null;
				break;
			}
			case ResponseType.ConnectionError:
			{
				reply.Status = ResponseType.ConnectionError;
				reply.Message = responseMessages[ResponseType.ConnectionError];
				reply.Data = null;
				break;
			}
			default:
			{
				reply.Status = ResponseType.UnknownError;
				reply.Message = responseMessages[ResponseType.UnknownError];
				reply.Data = null;
				break;
			}
		}
		
		return reply;
	}
	

	private DataJson GetJson()
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
		
		DataProcessor dataProcessor = new DataProcessor();
		FrameInfo digestedInfo = new FrameInfo();
		DataJson internalDataJson = new DataJson
		{
			Status = ResponseType.UnknownError,
			Data = sillyCat
		};
		
		const string inverterIp = "192.168.2.211";
		const int inverterPort = 8899;
		
		//TCP client
		using (TcpClient tcpClient = new TcpClient())
		{
			try
			{
				tcpClient.Connect(inverterIp, inverterPort);

				if (verbose) Console.WriteLine("Connected to the server.");
				int responses = 0;
				List<byte[]> bufferList = new List<byte[]>();
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

					bufferList.Add(buffer);

					responses++;
				}
				
				try
				{
					digestedInfo = dataProcessor.DigestResponse(bufferList);
				}
				catch (Exception e)
				{
					Console.WriteLine("Data processor error:\n"+e);
					// throw;
				}


				internalDataJson.Data = JsonSerializer.Serialize(digestedInfo) ?? sillyCat; // BitConverter.ToString(buffer).Replace("-", " ");
				internalDataJson.Status = ResponseType.Ok;
			}
			catch (Exception ex)
			{
				Console.WriteLine("[TCP Client] Error: " + ex.Message);
				Console.WriteLine(ex);
				if (ex.Message == "connection timeout or something")
				{
					internalDataJson.Data = ex.Message;
					internalDataJson.Status = ResponseType.ConnectionError;
				}
				else
				{
					internalDataJson.Data = ex.Message;
					internalDataJson.Status = ResponseType.InternalError;
				}
				
			}
		}

		// This point is reached upon error in TCP client.
		return internalDataJson;
	}

	private DataJson GetCachedJson()
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

		DataJson dataJson = new DataJson
		{
			Status = ResponseType.Ok,
			Data = otherSillyCat
		};
		return dataJson;
	}
	

}