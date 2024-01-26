using System.Text;
using System.Text.Json;

namespace InterServer.Logic;

public class DataProcessor
{
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
    
    private static int ConvertFrameHexValueToInt(string hexval, string reg)
    {
	    if (hexval == "" || hexval.Contains(" "))
	    {
		    Console.WriteLine("No value in response for register " + reg);
		    return 0;
	    }
	    int bits = 16;
	    int val = Convert.ToInt32(hexval, bits);
            
	    if ((val & (1 << (bits - 1))) != 0)
	    {
		    val -= 1 << bits;
	    }
            
	    return val;
    }

    private FrameInfo CheckFrameInfoJson(FrameInfo[] previousFramesArr)
    {
	    
	    return null;
    }
	public byte[] ConstructFrame(int sequence = 1)
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

	public FrameInfo DigestResponse(byte[] frame, FrameInfo previousFrame = null)
	{
		FrameInfo frameInfo = previousFrame ?? new FrameInfo();
		
		// Some example values, so it's not completely useless
		// TODO: make a proper frame analysis
		
		frameInfo.Fault3 = 12851;
		frameInfo.Fault4 = 12337;
		frameInfo.Fault5 = 12596;
		
		// Those are taken from the config file, no touchy
		byte[] pini = {0x0003};
		byte[] pfin = {0x0070};
		byte[] pini2 = {0x0096};
		byte[] pfin2 = {0x00f8};
		
		int processingIterations = 0;
		int i = Convert.ToInt32(0x0070) - Convert.ToInt32(0x0003); // Taken from the example code, no idea what that actually is
		
		while (processingIterations<=i)
		{
			int p1 = 56 + (processingIterations * 4);
            int p2 = 60 + (processingIterations * 4);
            string hexpos = $"0x{((processingIterations + 0x0003) & 0xFFFF):X4}";
            // Console.WriteLine("hexpos="+hexpos);
            
            string hexString = string.Concat(
	            BitConverter.ToString(frame).Replace("-", ""),
	            " ",
	            new string(Encoding.ASCII.GetString(frame).Where(c => c >= 0x20 && c <= 0x7F).ToArray()));
            string selectedSubstring = hexString.Substring(p1, p2 - p1);
            int intFrameValue = ConvertFrameHexValueToInt(selectedSubstring, hexpos);
            
            // Read the config, and get register position addresses 
            // TODO: switch to production-ready config file before continuing the development
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
		            string title = itemElement.GetProperty("titleEN").GetString();
		            string unit = itemElement.GetProperty("unit").GetString(); // Might be left unused for now
		            decimal ratio = itemElement.GetProperty("ratio").GetDecimal();
		            
		            // Iterate through the "registers" array
		            var registersArrayEnumerator = itemElement.GetProperty("registers").EnumerateArray();
		            foreach (JsonElement registerElement in registersArrayEnumerator)
		            {

			            bool found = registerElement.GetString().Contains(hexpos);
			            
			            // TODO: implement 2-byte value processing
			            if (registersArrayEnumerator.Count() > 1)
			            {
				            // Processing multi-byte registers
				            var jsonElementsArray = registersArrayEnumerator.ToArray();
				            if (found) Console.WriteLine($"Title: \"{title}\", registers: {string.Join("; ",jsonElementsArray)}, value: NIY");
			            }
			            else
			            {
				            // Processing single-byte registers
				            if (found)
				            {
								Console.WriteLine($"Title: \"{title}\", registers: {registerElement}, value: {intFrameValue*ratio}{unit}");
				            }
			            }
		            }
	            }
            }
            processingIterations++;
		}
		return frameInfo;
	}
}