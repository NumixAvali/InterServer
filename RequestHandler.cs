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

		byte[] frame = ConstructFrame();
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
					tcpClient.GetStream().Write(ConstructFrame(responses), 0, ConstructFrame(responses).Length);

					if (verbose) Console.WriteLine($"Message #{responses} was sent to the inverter");

					// Receive data from the server
					byte[] buffer = new byte[1024];
					int bytesRead = tcpClient.GetStream().Read(buffer, 0, buffer.Length);

					if (verbose)
					{
						Console.WriteLine("Reply from the inverter, length: " + bytesRead + " bytes");
						Console.WriteLine(Convert.ToBase64String(buffer));
					}

					digestedInfo = DigestResponse(buffer);

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
	
	private static byte[] ConvertHexToByteArray(string hexString)
	{
		int length = hexString.Length;
		byte[] byteArray = new byte[length / 2];

		for (int i = 0; i < length; i += 2)
		{
			if (hexString[i] == '\\' && hexString[i + 1] == 'x')
			{
				// Skip the '\x' part and convert the remaining two characters
				i += 2;
			}

			byteArray[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
		}

		return byteArray;
	}

	private byte[] ConstructFrame(int sequence = 1)
	{
		/*
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
		*/
		
		
		// Those are taken from the original python script
		// Eventually they need to be calculated by ConstructFrame()
		byte[] unbased1 = Convert.FromBase64String("pRcAEEUAADKt3qMCAAAAAAAAAAAAAAAAAAABAwADAG40Jp0V");
		byte[] unbased2 = Convert.FromBase64String("pRcAEEUAADKt3qMCAAAAAAAAAAAAAAAAAAABAwCWAGPlz38V");

		if (sequence == 0)
		{
			return unbased1;
		}
		else
		{
			return unbased2;
		}
		
		// return frame;
	}

	private FrameInfo DigestResponse(byte[] frame, FrameInfo previousFrame = null)
	{
		FrameInfo frameInfo = previousFrame ?? new FrameInfo();
		
		// Some example values, so it's not completely useless
		// TODO: make a proper frame analysis
		Random r = new Random();
		
		frameInfo.Fault3 = 12851;
		frameInfo.Fault4 = 12337;
		frameInfo.Fault5 = r.Next(10000, 13000);//12596;
		// frameInfo.TotalProduction = r.Next(0, 100);
		
		// byte[] interesting = { frame[56],frame[57],frame[58],frame[59],frame[60] };
		// Console.WriteLine(BitConverter.ToString(interesting).Replace("-", " "));


		// Those are taken from the config file, no touchy
		byte[] pini = {0x0003};
		byte[] pfin = {0x0070};
		byte[] pini2 = {0x0096};
		byte[] pfin2 = {0x00f8};

		int chunks = 1;
		
		int a = 0;
		int i = Convert.ToInt32(0x0070) - Convert.ToInt32(0x0003);

		while (a<=i)
		{
			int p1 = 56 + (a * 4);
            int p2 = 60 + (a * 4);
            string hexpos = $"0x{((a + 0x0003) & 0xFFFF):X4}";
            // Console.WriteLine("hexpos="+hexpos);
  
            // Read the config, and get register position addresses 
            string configFile = File.ReadAllText("./SOFARMap.json", Encoding.UTF8);
            JsonDocument jsonDocument = JsonDocument.Parse(configFile);
            
            var rootArrayEnumerator = jsonDocument.RootElement.EnumerateArray();

            // Iterate through the object array
            foreach (JsonElement element in rootArrayEnumerator)
            {
	            // Iterate through the "items" array
	            var itemsArrayEnumerator = element.GetProperty("items").EnumerateArray();
	            foreach (JsonElement itemElement in itemsArrayEnumerator)
	            {
		            // Access properties of each item
		            // Can be done in the scope with the rest of the logic as well tbh
		            string titleEN = itemElement.GetProperty("titleEN").GetString();
		            
		            // Iterate through the "registers" array
		            var registersArrayEnumerator = itemElement.GetProperty("registers").EnumerateArray();
		            foreach (JsonElement registerElement in registersArrayEnumerator)
		            {
			            bool found = registerElement.GetString().Contains(hexpos);
			            if (found) Console.WriteLine($"Title: \"{titleEN}\", registers: {registerElement}");
			            
			            // Here be valid frame processing
			            // TODO: implement frame processing
		            }
	            }
            }
  
            a++;
		}
		
		
		return frameInfo;
	}
}