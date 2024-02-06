namespace InterServer.Logic;

public enum JsonRequestType : ushort
{
	Get = 0,
	Test = 1
}

public enum ResponseType : ushort
{
	Ok = 0,
	AuthReject = 1,
	ServiceUnavailable = 2,
	IncorrectJson = 3,
	UnknownError = 4,
	Rejected = 5,
	InternalError = 6,
	ConnectionError = 7,
	InverterConfigError = 8,
	AppConfigError = 9,
	InvalidTimestamp = 10
}

public enum ReplyDataType : ushort
{
	CurrentData = 0,
	CachedLatestData = 1,
	CachedPeriodData = 2,
	NoData = 3
}


public class RequestJson
{
	public long Timestamp { get; set; }
	public string Token { get; set; }
}

public class ReplyJson
{
	public required ResponseType Status { get; set; }
	public required string Message { get; set; }
	public long Timestamp { get; set; }
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
	public string? Unit { get; set; }
	public float Scale { get; set; }
	public float Value { get; set; }
}

public class FrameInfo
{
	public InnerFrameInfo BatteryStatus { get; set; }
	public InnerFrameInfo BatteryCurrent { get; set; }
	public InnerFrameInfo LoadVoltage { get; set; }
	public InnerFrameInfo SmartLoadEnableStatus { get; set; }
	public InnerFrameInfo GridConnectedStatus { get; set; }
	public InnerFrameInfo UsageTime { get; set; }
	public InnerFrameInfo Alert { get; set; }
	public InnerFrameInfo WorkMode { get; set; }
	public InnerFrameInfo CommunicationBoardVersion { get; set; }
	public InnerFrameInfo ControlBoardVersion { get; set; }
	public InnerFrameInfo InverterStatus { get; set; }
	public InnerFrameInfo DailyBatteryCharge { get; set; }
	public InnerFrameInfo DailyBatteryDischarge { get; set; }
	public InnerFrameInfo TotalBatteryCharge { get; set; }
	public InnerFrameInfo TotalBatteryDischarge { get; set; }
	public InnerFrameInfo DailyEnergyBought { get; set; }
	public InnerFrameInfo DailyEnergySold { get; set; }
	public InnerFrameInfo TotalEnergyBought { get; set; }
	public InnerFrameInfo TotalEnergySold { get; set; }
	public InnerFrameInfo GridFrequency { get; set; }
	public InnerFrameInfo DailyLoadConsumption { get; set; }
	public InnerFrameInfo TotalLoadConsumption { get; set; }
	public InnerFrameInfo DcTemperature { get; set; }
	public InnerFrameInfo AcTemperature { get; set; }
	public InnerFrameInfo TotalProduction { get; set; }
	public InnerFrameInfo DailyProduction { get; set; }
	public InnerFrameInfo GridL1Current { get; set; }
	public InnerFrameInfo GridL2Current { get; set; }
	public InnerFrameInfo LoadL1Power { get; set; }
    public InnerFrameInfo LoadL2Power { get; set; }
    public InnerFrameInfo InverterL1Power { get; set; }
    public InnerFrameInfo InverterL2Power { get; set; }
	public InnerFrameInfo InternalL1LoadPower { get; set; }
	public InnerFrameInfo InternalL2LoadPower { get; set; }
	public InnerFrameInfo ExternalL1LoadPower { get; set; }
	public InnerFrameInfo ExternalL2LoadPower { get; set; }
	public InnerFrameInfo GridL1Voltage { get; set; }
	public InnerFrameInfo GridL2Voltage { get; set; }
	public InnerFrameInfo GenPower { get; set; }
	public InnerFrameInfo GenConnectedStatus { get; set; }
	public InnerFrameInfo MicroInverterPower { get; set; }
	public InnerFrameInfo BatterySoc { get; set; }
	public InnerFrameInfo Pv1Voltage { get; set; }
	public InnerFrameInfo Pv1Current { get; set; }
	public InnerFrameInfo Pv1Power { get; set; }
	public InnerFrameInfo Pv2Power { get; set; }
	public InnerFrameInfo Pv2Voltage { get; set; }
	public InnerFrameInfo Pv2Current { get; set; }
	public InnerFrameInfo BatteryPower { get; set; }
}
