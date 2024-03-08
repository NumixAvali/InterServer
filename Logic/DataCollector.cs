using InterServer.Controllers;

namespace InterServer.Logic;

public class DataCollector : BackgroundService
{
    private readonly ILogger<DataCollector> _logger;

    public DataCollector(ILogger<DataCollector> logger)
    {
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            RequestHandler requestHandler = new RequestHandler();
            AppSettings appSettings = new SettingsController().GetSettings();
            
            bool isConfigSafe = appSettings.GetType().GetProperties()
                .All(p => p.GetValue(appSettings) != null);
            
            if (appSettings.EnableAutomaticDataGather && isConfigSafe)
            {
                DbHandler dbHandler = new DbHandler(
                    appSettings.DbIp,
                    appSettings.DbName,
                    appSettings.DbUsername,
                    appSettings.DbPassword
                );

                dbHandler.UploadData(requestHandler.ResponseManager(ResponseType.Ok, ReplyDataType.CurrentData));

                await Task.Delay(Convert.ToInt32(appSettings.AutomaticGatherInterval) * Convert.ToInt32(appSettings.AutomaticGatherIntervalModifier), stoppingToken);
            }
            else
            {
                // Console.WriteLine("Data gathering disabled");
                Thread.Sleep(1000);
            }
        }
    }

}