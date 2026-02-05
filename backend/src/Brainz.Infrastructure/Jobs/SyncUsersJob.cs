using Brainz.Application.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Brainz.Infrastructure.Jobs;

public class SyncUsersJob
{
    private readonly ISyncService _syncService;
    private readonly ILogger<SyncUsersJob> _logger;

    public SyncUsersJob(ISyncService syncService, ILogger<SyncUsersJob> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 2)]
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting user sync job");
        await _syncService.SyncUsersAsync();
        _logger.LogInformation("Completed user sync job");
    }
}
