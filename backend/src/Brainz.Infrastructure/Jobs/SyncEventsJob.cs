using Brainz.Application.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Brainz.Infrastructure.Jobs;

public class SyncEventsJob
{
    private readonly ISyncService _syncService;
    private readonly ILogger<SyncEventsJob> _logger;

    public SyncEventsJob(ISyncService syncService, ILogger<SyncEventsJob> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 2)]
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting events sync job");
        await _syncService.SyncEventsAsync();
        _logger.LogInformation("Completed events sync job");
    }
}
