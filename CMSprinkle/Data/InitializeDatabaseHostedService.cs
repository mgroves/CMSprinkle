using System.Threading;
using System.Threading.Tasks;
using CMSprinkle.Data;
using Microsoft.Extensions.Hosting;

namespace CMSprinkle.Infrastructure;

public class InitializeDatabaseHostedService : IHostedService
{
    private readonly ICMSprinkleDataService _dataService;
    private bool _hasRun = false;

    public InitializeDatabaseHostedService(ICMSprinkleDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_hasRun)
        {
            await RunOnceAsync();
            _hasRun = true;
        }
    }

    private async Task RunOnceAsync()
    {
        await _dataService.InitializeDatabase();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}