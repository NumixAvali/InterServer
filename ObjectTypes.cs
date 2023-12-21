namespace InterServer;

public enum JsonRequestType : ushort
{
	Get = 0,
	Test = 1
}

public enum ResponseType : ushort
{
	IncorrectJson = 0,
	AuthReject = 1,
	ServiceUnavailable = 2,
	UnknownError = 3,
	Ok = 4,
	Rejected = 5
}

public enum ReplyDataType : ushort
{
	CurrentData = 0,
	CachedData = 1,
	NoData = 2
}


public class RequestJson
{
	public bool? ForceRefresh { get; set; }
	public string? Token { get; set; }
	public ResponseType RequestType { get; set; } 
}

public class ReplyJson
{
	public string message { get; set; }
	public string? data { get; set; }
}

public class TestJsonData
{
	public string? key1 { get; set; }
	public string? key2 { get; set; }
}

public class FrameInfo
{
	public int Fault3 { get; set; }
	public int Fault4 { get; set; }
	public int Fault5 { get; set; }
	public float OutputReactivePower { get; set; }
	public float L1Voltage { get; set; }
	public float L2Voltage { get; set; }
	public float L3Voltage { get; set; }
	public float L3Current { get; set; }
	public int TotalGenerationTimeHours { get; set; }
	public int TotalGenerationTimeMinutes { get; set; }
	public float BusVoltage { get; set; }
	public float Pv1VoltageSample { get; set; }
	public float Pv1CurrentSample { get; set; }
	public int CountdownTime { get; set; }
	public int InverterAlertMessage { get; set; }
	public int InputMode { get; set; }
	public int CommunicationBoardInnerMessage { get; set; }
	public int InsulationPv1PositiveGnd { get; set; }
	public int InsulationPv2PositiveGnd { get; set; }
	public int InsulationPvNegativeGnd { get; set; }
	public int Country { get; set; }
	public string InverterStatus { get; set; }
	public float DailyBatteryCharge { get; set; }
	public float DailyBatteryDischarge { get; set; }
	public float GridFrequency { get; set; }
	public float DailyConsumption { get; set; }
	public float TotalConsumption { get; set; }
	public float DcTemperature { get; set; }
	public float AcTemperature { get; set; }
	public float TotalProduction { get; set; }
	public float DailyProduction { get; set; }
	public float Pv1Voltage { get; set; }
	public float Pv1Current { get; set; }
	public float Pv2Voltage { get; set; }
	public float Pv2Current { get; set; }
	public float L1Current { get; set; }
	public float L2Current { get; set; }
	public float L1LoadPower { get; set; }
	public float L2LoadPower { get; set; }
	public int OutputActivePower { get; set; }
	public int BatterySoc { get; set; }
	public int Pv1Power { get; set; }
	public int Pv2Power { get; set; }
	public int BatteryPower { get; set; }
}
