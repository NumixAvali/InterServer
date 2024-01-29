namespace InterServer.Logic;

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
	Rejected = 5,
	InternalError = 6,
	ConnectionError = 7
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
	public required ResponseType Status { get; set; }
	public required string Message { get; set; }
	public FrameInfo? Data { get; set; }
}

public class DataJson
{
	public required ResponseType Status { get; set; }
	public required string Data { get; set; }
}

public class InnerFrameInfo
{
	public string Title { get; set; }
	public string Unit { get; set; }
	public float Scale { get; set; }
	public float Value { get; set; }
}

public class FrameInfo
{
	// TODO: turn every property into an object with value, data unit, and value modifier 
	public InnerFrameInfo Fault1 { get; set; }
	public InnerFrameInfo Fault2 { get; set; }
	public InnerFrameInfo Fault3 { get; set; }
	public InnerFrameInfo Fault4 { get; set; }
	public InnerFrameInfo Fault5 { get; set; }
	public InnerFrameInfo OutputReactivePower { get; set; }
	public InnerFrameInfo L1Voltage { get; set; }
	public InnerFrameInfo L2Voltage { get; set; }
	public InnerFrameInfo L3Voltage { get; set; }
	public InnerFrameInfo L3Current { get; set; }
	public InnerFrameInfo TotalGenerationTimeHours { get; set; }
	public InnerFrameInfo TotalGenerationTimeMinutes { get; set; }
	public InnerFrameInfo BusVoltage { get; set; }
	public InnerFrameInfo Pv1VoltageSample { get; set; }
	public InnerFrameInfo Pv1CurrentSample { get; set; }
	public InnerFrameInfo CountdownTime { get; set; }
	public InnerFrameInfo InverterAlertMessage { get; set; }
	public InnerFrameInfo InputMode { get; set; }
	public InnerFrameInfo CommunicationBoardInnerMessage { get; set; }
	public InnerFrameInfo InsulationPv1PositiveGnd { get; set; }
	public InnerFrameInfo InsulationPv2PositiveGnd { get; set; }
	public InnerFrameInfo InsulationPvNegativeGnd { get; set; }
	public InnerFrameInfo Country { get; set; }
	public InnerFrameInfo InverterStatus { get; set; }
	public InnerFrameInfo DailyBatteryCharge { get; set; }
	public InnerFrameInfo DailyBatteryDischarge { get; set; }
	public InnerFrameInfo GridFrequency { get; set; }
	public InnerFrameInfo DailyConsumption { get; set; }
	public InnerFrameInfo TotalConsumption { get; set; }
	public InnerFrameInfo DcTemperature { get; set; }
	public InnerFrameInfo AcTemperature { get; set; }
	public InnerFrameInfo TotalProduction { get; set; }
	public InnerFrameInfo DailyProduction { get; set; }
	public InnerFrameInfo Pv1Voltage { get; set; }
	public InnerFrameInfo Pv1Current { get; set; }
	public InnerFrameInfo Pv2Voltage { get; set; }
	public InnerFrameInfo Pv2Current { get; set; }
	public InnerFrameInfo L1Current { get; set; }
	public InnerFrameInfo L2Current { get; set; }
	public InnerFrameInfo L1LoadPower { get; set; }
	public InnerFrameInfo L2LoadPower { get; set; }
	public InnerFrameInfo OutputActivePower { get; set; }
	public InnerFrameInfo BatterySoc { get; set; }
	public InnerFrameInfo Pv1Power { get; set; }
	public InnerFrameInfo Pv2Power { get; set; }
	public InnerFrameInfo BatteryPower { get; set; }
}
