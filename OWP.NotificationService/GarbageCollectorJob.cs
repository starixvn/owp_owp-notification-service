namespace OWP.NotificationService;

using Olympus.Diagnostic;

public class GarbageCollectorJob : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var gc = GarbageCollector.CollectExtremely();
            Console.WriteLine(gc);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}