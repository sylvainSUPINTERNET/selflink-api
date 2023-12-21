

namespace Selflink_api.Services.Hosted;

public class StripeWebHookHostedService : BackgroundService
{
    private readonly ILogger<StripeWebHookHostedService> _logger;

    public IServiceProvider Services { get; }

    public StripeWebHookHostedService(ILogger<StripeWebHookHostedService> logger, IServiceProvider services)
    {
        _logger = logger;
        Services = services;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Consume Scoped Service Hosted Service running.");

        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Consume Scoped Service Hosted Service is working.");

        using (var scope = Services.CreateScope())
        {
            var scopedProcessingService = 
                scope.ServiceProvider
                    .GetRequiredService<IScopedProcessingService>();

            await scopedProcessingService.DoWork(stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Consume Scoped Service Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
}