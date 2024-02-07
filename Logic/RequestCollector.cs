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

            await Task.Delay(5000, stoppingToken);
        }
    }

}