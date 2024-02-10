namespace InterServer.Logic;

public class RequestCollector : BackgroundService
{
    private readonly ILogger<RequestCollector> _logger;

    public RequestCollector(ILogger<RequestCollector> logger)
    {
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // TODO: Implement DB interaction
            // Console.WriteLine("Worker running at: {0}", DateTimeOffset.Now);

            RequestHandler requestHandler = new RequestHandler();

            DbHandler dbHandler = new DbHandler(
                "192.168.2.116",
                "Measurements",
                "dbadmin",
                ""
                );
            
            dbHandler.UploadData(requestHandler.ResponseManager(ResponseType.Ok,ReplyDataType.CurrentData));

            await Task.Delay(1*60*1000, stoppingToken);
        }
    }

}