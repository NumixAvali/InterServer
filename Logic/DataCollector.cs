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
        AppSettings settings = new SettingsController().GetSettings();

        if (!settings.EnableAutomaticDataGather) return;
            
        while (!stoppingToken.IsCancellationRequested)
        {
            // TODO: Implement DB interaction
            // Console.WriteLine("Worker running at: {0}", DateTimeOffset.Now);

            RequestHandler requestHandler = new RequestHandler();
            AppSettings appSettings = new SettingsController().GetSettings();
            
            bool isConfigSafe = appSettings.GetType().GetProperties()
                .All(p => p.GetValue(appSettings) != null);
            
            if (!isConfigSafe) return;

            DbHandler dbHandler = new DbHandler(
                appSettings.DbIp,
                appSettings.DbName,
                appSettings.DbUsername,
                appSettings.DbPassword
                );
            
            dbHandler.UploadData(requestHandler.ResponseManager(ResponseType.Ok,ReplyDataType.CurrentData));

            // TODO: finish making setting menu, so this horror can be avoided
            await Task.Delay(10*60*1000, stoppingToken);
        }
    }

}