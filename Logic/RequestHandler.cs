using System.Net.Sockets;
using System.Text.Json;

namespace InterServer.Logic;

public class RequestHandler
{
	public ReplyJson ResponseManager(ResponseType responseType, ReplyDataType dataType = ReplyDataType.NoData, DataJson? additionalData = null)
	{
		DataJson preFinalJson = new DataJson()
		{
			Status = ResponseType.UnknownError,
			Data = @"⣿⣿⡟⡹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
⣿⣿⢱⣶⣭⡻⢿⠿⣛⣛⣛⠸⣮⡻⣿⣿⡿⢛⣭⣶⣆⢿⣿
⣿⡿⣸⣿⣿⣿⣷⣮⣭⣛⣿⣿⣿⣿⣶⣥⣾⣿⣿⣿⡷⣽⣿
⣿⡏⣾⣿⣿⡿⠿⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⣿⣿
⣿⣧⢻⣿⡟⣰⡿⠁⢹⣿⣿⣿⣋⣴⠖⢶⣝⢻⣿⣿⡇⣿⣿
⠩⣥⣿⣿⣴⣿⣇⠀⣸⣿⣿⣿⣿⣷⠀⢰⣿⠇⣿⣭⣼⠍⣿
⣿⡖⣽⣿⣿⣿⣿⣿⣿⣯⣭⣭⣿⣿⣷⣿⣿⣿⣿⣿⡔⣾⣿
⣿⡡⢟⡛⠻⠿⣿⣿⣿⣝⣨⣝⣡⣿⣿⡿⠿⠿⢟⣛⣫⣼⣿
⣿⣿⣿⡷⠝⢿⣾⣿⣿⣿⣿⣿⣿⣿⣿⣾⡩⣼⣿⣿⣿⣿⣿
⣿⣿⣯⡔⢛⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣭⣍⣨⠿⢿⣿⣿⣿",
		};
		
		var reply = new ReplyJson
		{
			Status = ResponseType.UnknownError,
			Message = "Internal error!",
			Timestamp = GetUnixTimestamp(DateTime.Now),
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
			{ ResponseType.InternalError, "Internal Error."},
			{ ResponseType.InvalidTimestamp, "Invalid Timestamp"},
			{ ResponseType.ConnectionError, "Connection Error."}
		};
		
		switch (dataType)
		{
			case ReplyDataType.CachedLatestData:
				preFinalJson = GetRecentCachedJson();
				break;
			case ReplyDataType.CachedPeriodData:
				if (additionalData != null)
				{
					preFinalJson = GetHistoricCachedJson(UnixTimeStampToDateTime(Convert.ToInt32(additionalData.Data)));
				}
				break;
			case ReplyDataType.CurrentData:
				preFinalJson = GetJson();
				break;
			// default:
			// 	throw new ArgumentException("Invalid reply data type.", nameof(dataType));
		}
		
		if (responseMessages.TryGetValue(responseType, out var message))
		{
			reply.Message = message;
		}
		
		// Special handling cases. Yes, it's bad
		switch (preFinalJson.Status)
		{
			case ResponseType.Ok:
			{
				reply.Status = ResponseType.Ok;
				reply.Message = responseMessages[ResponseType.Ok];
				try
				{
					reply.Data = JsonSerializer.Deserialize<FrameInfo>(preFinalJson.Data) ;
				}
				catch (Exception e)
				{
					Console.WriteLine("[Request Handler] JSON casting to data field error");
					Console.WriteLine(e);
					// throw;
					reply.Status = ResponseType.InternalError;
					reply.Message = responseMessages[ResponseType.InternalError];
					reply.Data = null;
				}
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
			case ResponseType.InvalidTimestamp:
			{
				reply.Status = ResponseType.InvalidTimestamp;
				reply.Message = responseMessages[ResponseType.InvalidTimestamp];
				reply.Data = null;
				break;
			}
			// This part is disabled, until I ran into a problem, which will force me to re-enable it back
			// default:
			// {
			// 	Console.WriteLine($"UNIMPLEMENTED CALL {reply.Status} TRIGGERED THIS DEFAULT PART");
			// 	reply.Status = ResponseType.UnknownError;
			// 	reply.Message = responseMessages[ResponseType.UnknownError];
			// 	reply.Data = null;
			// 	break;
			// }
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
					tcpClient.GetStream().Write(dataProcessor.ConstructFrameRequest(responses), 0, dataProcessor.ConstructFrameRequest(responses).Length);

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
				if (ex.Message.Contains("connection"))
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

	private DataJson GetHistoricCachedJson(DateTime timePeriod)
	{
		long[] validTimestamps = { 621, 926, 69, 420 };

		long stamp = GetUnixTimestamp(timePeriod);

		DataJson dataJson = new DataJson
		{
			Status = ResponseType.Ok,
			Data =
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
⠀⠀⠀⠀⠀⠀⢿⠀⠀⠀⠀⠀⢸⡇⠀⠀⢹⡏⠁⠀⠀⠀⠀⠀⠀"
		};

		if (!validTimestamps.Contains(stamp))
		{
			dataJson.Status = ResponseType.InvalidTimestamp;
			// dataJson.Data = null;
			return dataJson;
		}

		// TODO: some DB requesting mechanism

		FrameInfo frameInfo = new FrameInfo();
		dataJson.Data = JsonSerializer.Serialize(frameInfo) ?? throw new InvalidOperationException();
		return dataJson;
	}

	private DataJson GetRecentCachedJson()
	{
		DataJson dataJson = new DataJson
		{
			Status = ResponseType.Ok,
			Data = 
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
⠀⠀⠀⠀⠀⠀⢿⠀⠀⠀⠀⠀⢸⡇⠀⠀⢹⡏⠁⠀⠀⠀⠀⠀⠀"
		};
		
		// TODO: some DB requesting mechanism

		FrameInfo frameInfo = new FrameInfo();
		dataJson.Data = JsonSerializer.Serialize(frameInfo);
		return dataJson;
	}
	private long GetUnixTimestamp(DateTime timeframe)
	{
		DateTimeOffset dateTimeOffset = new DateTimeOffset(timeframe);
		return dateTimeOffset.ToUnixTimeSeconds();
	}
	
	public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
	{
		// Unix timestamp is seconds past epoch
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
		return dateTime;
	}
}