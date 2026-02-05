using Brainz.Application.DTOs;
using Brainz.Application.Interfaces;
using Brainz.Domain.Enums;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brainz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;

    public SyncController(ISyncService syncService) => _syncService = syncService;

    [HttpGet("logs")]
    [ProducesResponseType(typeof(IEnumerable<SyncLogDto>), 200)]
    public async Task<IActionResult> GetSyncLogs()
    {
        var logs = await _syncService.GetRecentSyncLogsAsync();
        return Ok(logs);
    }

    [HttpPost("trigger")]
    [ProducesResponseType(200)]
    public IActionResult TriggerSync([FromQuery] string type = "Full")
    {
        if (!Enum.TryParse<SyncType>(type, true, out var syncType))
            return BadRequest(new { message = "Tipo inválido. Use: Users, Events, Full" });

        BackgroundJob.Enqueue<ISyncService>(s => s.TriggerManualSyncAsync(syncType));
        return Ok(new { message = $"Sincronização '{type}' adicionada à fila." });
    }
}
