using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using InterServer.Controllers;

namespace InterServer.Logic;

public class RequestHandler
{
	public dynamic ResponseManager(ResponseType responseType, ReplyDataType dataType = ReplyDataType.NoData, DataJson? additionalData = null)
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
		
		var replyJsonRegular = new ReplyJson
		{
			Status = ResponseType.UnknownError,
			Message = "Internal error!",
			Timestamp = GetUnixTimestamp(DateTime.Now),
			Data = null
		};
		var replyJsonNested = new ReplyJsonNested
		{
			Status = ResponseType.UnknownError,
			Message = "Internal error!",
			Timestamp = GetUnixTimestamp(DateTime.Now),
			Data = null
		};
		var replyJsonList = new ReplyJsonList
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
			{ ResponseType.ConnectionError, "Connection Error."},
			{ ResponseType.NoDataAvailableYet, "No data available yet, try again later."},
			{ ResponseType.NotEnoughData, "Not enough stored data to fulfil request."}
		};
		
		switch (dataType)
		{
			case ReplyDataType.CachedLatestData:
				preFinalJson = GetRecentCachedJson();
				break;
			case ReplyDataType.CachedPeriodData:
				if (additionalData != null)
				{
					preFinalJson = GetHistoricCachedJson(Convert.ToUInt32(additionalData.Data));
				}
				break;
			case ReplyDataType.CurrentData:
				preFinalJson = GetJson();
				// This hack is a very simple and sporadic value validator.
				// Because sometimes for unknown reason reply might consist of a bunch of empty values,
				// which ruin the whole point of the project.
				FrameInfo tempCheck = preFinalJson.Data; //JsonSerializer.Deserialize<FrameInfo>(preFinalJson.Data)!;
				while 
				(
					tempCheck.BatteryStatus.Value == 0 &&
					tempCheck.BatteryCurrent.Value == 0 &&
					tempCheck.SmartLoadEnableStatus.Value == 0 &&
					tempCheck.LoadVoltage.Value == 0 &&
					tempCheck.GridConnectedStatus.Value == 0 &&
					tempCheck.UsageTime.Value == 0 &&
					tempCheck.BatterySoc.Value == 0
				)
				{
					Console.WriteLine("[Error correction] Invalid data frame received, requesting next one.");
					preFinalJson = GetJson();
					tempCheck = preFinalJson.Data; //JsonSerializer.Deserialize<FrameInfo>(preFinalJson.Data)!;
				}

				break;
			case ReplyDataType.CachedRangeData:
				if (additionalData != null)
				{
					preFinalJson = GetHistoricCachedJsonRange(additionalData.Data[0], additionalData.Data[1]);
				}
				break;
			case ReplyDataType.AllData:
				preFinalJson = GetAllDbEntries();
				break;
			case ReplyDataType.Timestamps:
				throw new NotImplementedException();
				break;
			// default:
			// 	throw new ArgumentException("Invalid reply data type.", nameof(dataType));
		}
		
		if (responseMessages.TryGetValue(responseType, out var message))
		{
			replyJsonRegular.Message = message;
			replyJsonNested.Message = message;
			replyJsonList.Message = message;
		}
		
		switch (preFinalJson.Status)
		{
			case ResponseType.Ok:
			{
				// Maybe there's a better way of doing that...
				if (preFinalJson.DataType == typeof(FrameInfo))
				{
					replyJsonRegular.Status = preFinalJson.Status;
					replyJsonRegular.Message = responseMessages[preFinalJson.Status];
					replyJsonRegular.Data = preFinalJson.Data;
				}
				else if (preFinalJson.DataType == typeof(ReplyJsonNested))
				{
					replyJsonNested.Status = preFinalJson.Status;
					replyJsonNested.Message = responseMessages[preFinalJson.Status];
					replyJsonNested.Data = preFinalJson.Data;
				}
				else if (preFinalJson.DataType == typeof(ReplyJsonList))
				{
					replyJsonList.Status = preFinalJson.Status;
					replyJsonList.Message = responseMessages[preFinalJson.Status];
					replyJsonList.Data = preFinalJson.Data;
				}
				else
				{
					// Default case
					Console.WriteLine("Unknown dataJson datatype reached!");
				}
				
				break;
			}
			case ResponseType.InternalError:
			case ResponseType.ConnectionError:
			case ResponseType.InvalidTimestamp:
			case ResponseType.NoDataAvailableYet:
			case ResponseType.NotEnoughData:
			case ResponseType.UnknownError:
			{
				if (preFinalJson.DataType == typeof(FrameInfo))
				{
					replyJsonRegular.Status = preFinalJson.Status;
					replyJsonRegular.Message = responseMessages[preFinalJson.Status];
					replyJsonRegular.Data = null;
				}
				else if (preFinalJson.DataType == typeof(ReplyJsonNested))
				{
					replyJsonNested.Status = preFinalJson.Status;
					replyJsonNested.Message = responseMessages[preFinalJson.Status];
					replyJsonNested.Data = null;
				}
				else if (preFinalJson.DataType == typeof(ReplyJsonList))
				{
					replyJsonList.Status = preFinalJson.Status;
					replyJsonList.Message = responseMessages[preFinalJson.Status];
					replyJsonList.Data = null;
				}
				
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
		
		if (preFinalJson.DataType == typeof(FrameInfo))
		{
			return replyJsonRegular;
		}
		else if (preFinalJson.DataType == typeof(ReplyJsonNested))
		{
			return replyJsonNested;
		}
		else if (preFinalJson.DataType == typeof(ReplyJsonList))
		{
			return replyJsonList;
		}

		return null;
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
		
		string inverterIp = new SettingsController().GetSettings().InverterIp;
		int inverterPort = new SettingsController().GetSettings().InverterPort;
		
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

				internalDataJson.Data = digestedInfo; //JsonSerializer.Serialize(digestedInfo) ?? sillyCat; // BitConverter.ToString(buffer).Replace("-", " ");
				internalDataJson.Status = ResponseType.Ok;
				internalDataJson.DataType = typeof(FrameInfo);
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

	private DataJson GetHistoricCachedJson(uint timestamp)
	{
		AppSettings settings = new SettingsController().GetSettings();
		ReplyJson dbEntry = new DbHandler(
			settings.DbIp,
			settings.DbName,
			settings.DbUsername,
			settings.DbPassword
		).GetDataByTimestamp(timestamp);
		
		DataJson dataJson = new DataJson
		{
			Status = dbEntry != null ? ResponseType.Ok : ResponseType.InvalidTimestamp,
			DataType = typeof(ReplyJsonNested),
			Data = dbEntry //JsonSerializer.Serialize(dbEntry)
		};

		return dataJson;
	}

	private DataJson GetRecentCachedJson()
	{
		AppSettings settings = new SettingsController().GetSettings();
		ReplyJson dbEntry = new DbHandler(
			settings.DbIp,
			settings.DbName,
			settings.DbUsername,
			settings.DbPassword
		).GetLatestData();
		
		DataJson dataJson = new DataJson
		{
			Status = dbEntry.Data != null ? ResponseType.Ok : ResponseType.NoDataAvailableYet,
			DataType = typeof(ReplyJsonNested),
			Data = dbEntry //JsonSerializer.Serialize(dbEntry)
		};
		
		return dataJson;
	}

	private DataJson GetHistoricCachedJsonRange(uint timestampStart, uint timestampEnd)
	{
		List<ReplyJson> dbEntries = new DbHandler().GetDataRange(timestampStart, timestampEnd);
		
		DataJson dataJson = new DataJson
		{
			Status = dbEntries?.Any() == true ? ResponseType.Ok : ResponseType.NotEnoughData,
			DataType = typeof(ReplyJsonList),
			Data = dbEntries
		};

		return dataJson;
	}

	private DataJson GetAllDbEntries()
	{
		List<ReplyJson> dbEntries = new DbHandler().GetAllData();
		
		DataJson dataJson = new DataJson
		{
			Status = dbEntries?.Any() == true ? ResponseType.Ok : ResponseType.NotEnoughData,
			DataType = typeof(ReplyJsonList),
			Data = dbEntries
		};

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