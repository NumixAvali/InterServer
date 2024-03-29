using System.Text;
using InterServer.Controllers;
using YamlDotNet.Serialization;

namespace InterServer.Logic;

public class DataProcessor
{
	private static ushort CalculateModbusCRC(byte[] data)
	{
		ushort crc = 0xFFFF;
		for (int pos = 0; pos < data.Length; pos++)
		{
			crc ^= (ushort)data[pos];

			for (int i = 8; i != 0; i--)
			{
				if ((crc & 0x0001) != 0)
				{
					crc >>= 1;
					crc ^= 0xA001;
				}
				else
					crc >>= 1;
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
    
    private static int ConvertFrameHexValueToInt(string hexval, string reg)
    {
	    if (hexval == "" || hexval.Contains(" "))
	    {
		    Console.WriteLine("No value in response for register " + reg);
		    return 0;
	    }
	    int bits = 16;
	    int val = Convert.ToInt32(hexval, bits);
	    
	    // No idea, what that is, and why is it needed!
	    // Original script had something like that, and I'm leaving it just in case.
	    if ((val & (1 << (bits - 1))) != 0)
	    {
		    val -= 1 << bits;
	    }
            
	    return val;
    }

	public byte[] ConstructFrameRequest(int sequence)
	{
		var config = ReadInverterConfig();
		
		// TODO: make that value configurable from GUI
		uint inverterSn = new SettingsController().GetSettings().SerialNumber;
		int regStart = config.requests[sequence].start;
		int regEnd = config.requests[sequence].end;
		
		// Data frame begin
		byte[] start = { 0xA5 };
		byte[] length = { 0x17, 0x00 }; 
		byte[] controlCode = { 0x10, 0x45 };
		byte[] serial = { 0x00, 0x00 };

		// Blank data field
		byte[] dataField = { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
		
		string posIni = regStart.ToString("X2").PadLeft(4, '0');
		string posFin = ((regEnd - regStart + 1) & 0xFF).ToString("X2").PadLeft(4, '0');
		
		// Calculate Business field
		byte[] businessField = { 0x01, 0x03,
			Convert.ToByte(posIni.Substring(0, 2), 16),
			Convert.ToByte(posIni.Substring(2, 2), 16),
			Convert.ToByte(posFin.Substring(0, 2), 16),
			Convert.ToByte(posFin.Substring(2, 2), 16) };

		// Stupid workaround, because algorithm produces that value with 1 extra bit
		businessField[5] -= Convert.ToByte(sequence);
		
		byte[] crc = BitConverter.GetBytes(CalculateModbusCRC(businessField)).ToArray();

		byte[] checksum = { 0x00 };
		byte[] endCode = { 0x15 };

		byte[] inverterSn2 = BitConverter.GetBytes(inverterSn);

		// Construct the frame
		byte[] frameRequest = ConcatArrays(start, length, controlCode, serial, inverterSn2, dataField, businessField, crc, checksum, endCode);
		
		// Calculating and filling in the "checksum" byte
		int frameChecksum = 0;
		for (int i = 1; i < frameRequest.Length - 2; i++)
		{
			frameChecksum += frameRequest[i];
		}
		frameRequest[frameRequest.Length - 2] = (byte)(frameChecksum & 0xFF);

		// Those are taken from the original python script
		// And were used only for comparison in certain cases 
		// byte[] unbased1 = Convert.FromBase64String("pRcAEEUAADKt3qMCAAAAAAAAAAAAAAAAAAABAwADAG40Jp0V");
		// byte[] unbased2 = Convert.FromBase64String("pRcAEEUAADKt3qMCAAAAAAAAAAAAAAAAAAABAwCWAGPlz38V");
		// string comparisonValue = Convert.ToBase64String(frameRequest);
		
		return frameRequest;
	}

	public FrameInfo DigestResponse(List<byte[]> frameList)
	{
		// This is clearly a bad practice, and probably can be done much better.
		// I don't really know how, and maybe at some point in the future I'll make it not terrible!
		var frameInfoTemp = new FrameInfo
		{
				GridL1Voltage = new InnerFrameInfo { Title = "Grid Voltage L1" },
				GridL2Voltage = new InnerFrameInfo { Title = "Grid Voltage L2" },
				LoadL1Power = new InnerFrameInfo { Title = "Load L1 Power" },
				LoadL2Power = new InnerFrameInfo { Title = "Load L2 Power" },
				InverterL1Power = new InnerFrameInfo { Title = "Inverter L1 Power" },
				InverterL2Power = new InnerFrameInfo { Title = "Inverter L2 Power" },
				BatteryStatus = new InnerFrameInfo { Title = "Battery Status" },
				BatteryCurrent = new InnerFrameInfo { Title = "Battery Current" },
				LoadVoltage = new InnerFrameInfo { Title = "Load Voltage" },
				SmartLoadEnableStatus = new InnerFrameInfo { Title = "SmartLoad Enable Status" },
				GridConnectedStatus = new InnerFrameInfo { Title = "Grid-connected Status" },
				UsageTime = new InnerFrameInfo { Title = "Time of use" },
				Alert = new InnerFrameInfo { Title = "Alert" },
				WorkMode = new InnerFrameInfo { Title = "Work Mode" },
				CommunicationBoardVersion = new InnerFrameInfo { Title = "Communication Board Version No." },
				ControlBoardVersion = new InnerFrameInfo { Title = "Control Board Version No." },
				InverterStatus = new InnerFrameInfo { Title = "Running Status" },
				DailyBatteryCharge = new InnerFrameInfo { Title = "Daily Battery Charge" },
				DailyBatteryDischarge = new InnerFrameInfo { Title = "Daily Battery Discharge" },
				TotalBatteryCharge = new InnerFrameInfo { Title = "Total Battery Charge" },
				TotalBatteryDischarge = new InnerFrameInfo { Title = "Total Battery Discharge" },
				DailyEnergyBought = new InnerFrameInfo { Title = "Daily Energy Bought" },
				DailyEnergySold = new InnerFrameInfo { Title = "Daily Energy Sold" },
				TotalEnergyBought = new InnerFrameInfo { Title = "Total Energy Bought" },
				TotalEnergySold = new InnerFrameInfo { Title = "Total Energy Sold" },
				GridFrequency = new InnerFrameInfo { Title = "Grid Frequency" },
				DailyLoadConsumption = new InnerFrameInfo { Title = "Daily Load Consumption" },
				TotalLoadConsumption = new InnerFrameInfo { Title = "Total Load Consumption" },
				DcTemperature = new InnerFrameInfo { Title = "DC Temperature" },
				AcTemperature = new InnerFrameInfo { Title = "AC Temperature" },
				TotalProduction = new InnerFrameInfo { Title = "Total Production" },
				DailyProduction = new InnerFrameInfo { Title = "Daily Production" },
				Pv1Voltage = new InnerFrameInfo { Title = "PV1 Voltage" },
				Pv1Current = new InnerFrameInfo { Title = "PV1 Current" },
				Pv2Voltage = new InnerFrameInfo { Title = "PV2 Voltage" },
				Pv2Current = new InnerFrameInfo { Title = "PV2 Current" },
				GridL1Current = new InnerFrameInfo { Title = "Grid Current L1" },
				GridL2Current = new InnerFrameInfo { Title = "Grid Current L2" },
				InternalL1LoadPower = new InnerFrameInfo { Title = "Internal CT L1 Power" },
				InternalL2LoadPower = new InnerFrameInfo { Title = "Internal CT L2 Power" },
				ExternalL1LoadPower = new InnerFrameInfo { Title = "External CT L1 Power" },
				ExternalL2LoadPower = new InnerFrameInfo { Title = "External CT L2 Power" },
				GenPower = new InnerFrameInfo { Title = "Gen Power" },
				GenConnectedStatus = new InnerFrameInfo { Title = "Gen-connected Status" },
				MicroInverterPower = new InnerFrameInfo { Title = "Micro-inverter Power" },
				BatterySoc = new InnerFrameInfo { Title = "Battery SOC" },
				Pv1Power = new InnerFrameInfo { Title = "PV1 Power" },
				Pv2Power = new InnerFrameInfo { Title = "PV2 Power" },
				BatteryPower = new InnerFrameInfo { Title = "Battery Power" }
		};
		
		
		// Read the config, and get register position addresses 
		var config = ReadInverterConfig(); //deserializer.Deserialize<RootObject>(configFile);
		
		// Those are taken from the config file, no touchy
		byte[] pini = {0x0003};
		byte[] pfin = {0x0070};
		byte[] pini2 = {0x0096};
		byte[] pfin2 = {0x00f8};
		
		int requestIterations = 0;

		foreach (byte[] frame in frameList)
		{
			var processingIterations = 0;
			var i = config.requests[requestIterations].end - config.requests[requestIterations].start;
			// Console.WriteLine("===FRAME START===");
			while (processingIterations <= i)
			{
				var p1 = 56 + processingIterations * 4;
				var p2 = 60 + processingIterations * 4;
				var hexpos = $"0x{(processingIterations + config.requests[requestIterations].start) & 0xFFFF:X4}";
				// Console.WriteLine("hexpos="+hexpos);

				var hexString = string.Concat(
					BitConverter.ToString(frame).Replace("-", ""),
					" ",
					new string(Encoding.ASCII.GetString(frame).Where(c => c >= 0x20 && c <= 0x7F).ToArray()));
				var selectedSubstring = hexString.Substring(p1, p2 - p1);
				var intFrameValue = ConvertFrameHexValueToInt(selectedSubstring, hexpos);
				
				// Config logging and processing
				foreach (var parameter in config.parameters)
				{
					// Console.WriteLine($"Group: {parameter.group}");
					
					foreach (var item in parameter.items)
					{
						if (item.registers.Count() > 1)
						{
							if (item.registers[0].Contains(hexpos))
							{ // Some special multi-byte code might be needed
								
								// Console.WriteLine($"Title: \"{item.name}\", registers: {String.Join("; ", item.registers)}, value: {intFrameValue * item.scale}{item.uom}");
								
								foreach (var property in frameInfoTemp.GetType().GetProperties())
								{
									// Get the nested InnerFrameInfo object
									InnerFrameInfo innerFrameInfo = (InnerFrameInfo)property.GetValue(frameInfoTemp);

									if (innerFrameInfo.Title == item.name)
									{
										// Console.WriteLine($"Found matching property: {property.Name} - multi-byte");
										innerFrameInfo.Value = intFrameValue;
										innerFrameInfo.Scale = item.scale;
										innerFrameInfo.Unit = item.uom;
									}
								}
							}
						}
						else
						{
							if (item.registers[0].Contains(hexpos))
							{
								// Console.WriteLine($"Title: \"{item.name}\", registers: {item.registers[0]}, value: {intFrameValue * item.scale}{item.uom} ({intFrameValue}{item.uom})");
								
								foreach (var property in frameInfoTemp.GetType().GetProperties())
								{
									// Get the nested InnerFrameInfo object
									InnerFrameInfo innerFrameInfo = (InnerFrameInfo)property.GetValue(frameInfoTemp);

									if (innerFrameInfo.Title == item.name)
									{
										// Console.WriteLine($"Found matching property: {property.Name} - single-byte");
										innerFrameInfo.Value = intFrameValue;
										innerFrameInfo.Scale = item.scale;
										innerFrameInfo.Unit = item.uom;
									}
								}
							}
						}
					}
				}
				processingIterations++;
			}
			// Console.WriteLine("===FRAME END===");
			requestIterations++;
		}
		

		return frameInfoTemp; //MergeFrameInfos(frameInfoList);
	}

	public YamlRootObject ReadInverterConfig()
	{
		string inverterConfigName = new SettingsController().GetSettings().ConfigName;
		string configFile = File.ReadAllText($"InverterConfigs/{inverterConfigName}");
				
		var deserializer = new DeserializerBuilder().Build();
		var config = deserializer.Deserialize<YamlRootObject>(configFile);
		
		return config;
	}
}