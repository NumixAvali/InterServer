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

		// This probably can be moved to JsonTypes.cs
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
		
		// This should be done via proper configuration menu, not hardcoded.
		// TODO: make a configuration menu for those
		uint inverterSn = 2749279538;
		int regStart1 = Convert.ToInt32("0x0003", 16);
		int regEnd1 = Convert.ToInt32("0x0070", 16);
		
		int pini = regStart1;
		int pfin = regEnd1;
		// Data frame begin
		byte[] start = { 0xA5 };
		byte[] length = { 0x17, 0x00 }; 
		byte[] controlCode = { 0x10, 0x45 };
		byte[] serial = { 0x00, 0x00 };

		// Blank data field
		byte[] dataField = { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

		string posIni = Convert.ToString(pini, 16).PadLeft(4, '0');
		string posFin = Convert.ToString(pfin - pini + 1, 16).PadLeft(4, '0');

		byte[] businessField = { 0x01, 0x03,
			Convert.ToByte(posIni.Substring(0, 2), 16),
			Convert.ToByte(posIni.Substring(2, 2), 16),
			Convert.ToByte(posFin.Substring(0, 2), 16),
			Convert.ToByte(posFin.Substring(2, 2), 16) };
		
		ushort crcValue = CalculateCrc16Modbus(businessField);
		byte[] crc = { (byte)(crcValue & 0xFF), (byte)((crcValue >> 8) & 0xFF) };

		byte[] checksum = { 0x00 }; // checksum F2
		byte[] endCode = { 0x15 };

		byte[] inverterSn2 = BitConverter.GetBytes(inverterSn);
		Array.Reverse(inverterSn2);

		// Construct the frame
		byte[] frame = ConcatArrays(start, length, controlCode, serial, inverterSn2, dataField, businessField, crc, checksum, endCode);
		
		// Console.WriteLine(BitConverter.ToString(frame).Replace("-", " "));

		
		return BitConverter.ToString(frame).Replace("-", " "); // sillyCat;
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
	
	private static ushort CalculateCrc16Modbus(byte[] data)
	{
		ushort crc = 0xFFFF;

		foreach (byte b in data)
		{
			crc ^= b;

			for (int i = 0; i < 8; i++)
			{
				if ((crc & 0x0001) != 0)
				{
					crc >>= 1;
					crc ^= 0xA001;
				}
				else
				{
					crc >>= 1;
				}
			}
		}

		return crc;
	}
	
	private static byte[] ConcatArrays(params byte[][] arrays)
	{
		int totalLength = arrays.Sum(array => array.Length);
		byte[] result = new byte[totalLength];
		int offset = 0;

		foreach (var array in arrays)
		{
			Buffer.BlockCopy(array, 0, result, offset, array.Length);
			offset += array.Length;
		}

		return result;
	}
}