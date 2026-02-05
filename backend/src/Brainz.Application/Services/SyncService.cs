using Brainz.Application.DTOs;
using Brainz.Application.Interfaces;
using Brainz.Domain.Entities;
using Brainz.Domain.Enums;
using Brainz.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Brainz.Application.Services;

public class SyncService : ISyncService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMicrosoftGraphService _graphService;
    private readonly ILogger<SyncService> _logger;

    public SyncService(
        IUnitOfWork unitOfWork,
        IMicrosoftGraphService graphService,
        ILogger<SyncService> logger)
    {
        _unitOfWork = unitOfWork;
        _graphService = graphService;
        _logger = logger;
    }

    public async Task SyncUsersAsync()
    {
        var syncLog = new SyncLog
        {
            Id = Guid.NewGuid(),
            SyncType = SyncType.Users,
            Status = SyncStatus.Running,
            StartedAt = DateTime.UtcNow
        };
        await _unitOfWork.SyncLogs.AddAsync(syncLog);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            var graphUsers = await _graphService.GetAllUsersAsync();
            int processed = 0, failed = 0;

            foreach (var graphUser in graphUsers)
            {
                try
                {
                    var existingUser = await _unitOfWork.Users
                        .GetByMicrosoftIdAsync(graphUser.MicrosoftId);

                    if (existingUser == null)
                    {
                        var newUser = new User
                        {
                            Id = Guid.NewGuid(),
                            MicrosoftId = graphUser.MicrosoftId,
                            DisplayName = graphUser.DisplayName,
                            Email = graphUser.Email,
                            GivenName = graphUser.GivenName,
                            Surname = graphUser.Surname,
                            Department = graphUser.Department,
                            JobTitle = graphUser.JobTitle,
                            IsActive = true,
                            LastSyncedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.Users.AddAsync(newUser);
                    }
                    else
                    {
                        existingUser.DisplayName = graphUser.DisplayName;
                        existingUser.Email = graphUser.Email;
                        existingUser.GivenName = graphUser.GivenName;
                        existingUser.Surname = graphUser.Surname;
                        existingUser.Department = graphUser.Department;
                        existingUser.JobTitle = graphUser.JobTitle;
                        existingUser.LastSyncedAt = DateTime.UtcNow;
                        _unitOfWork.Users.Update(existingUser);
                    }
                    processed++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to sync user {MicrosoftId}", graphUser.MicrosoftId);
                    failed++;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            syncLog.Status = failed > 0 ? SyncStatus.CompletedWithErrors : SyncStatus.Completed;
            syncLog.CompletedAt = DateTime.UtcNow;
            syncLog.RecordsProcessed = processed;
            syncLog.RecordsFailed = failed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User sync failed");
            syncLog.Status = SyncStatus.Failed;
            syncLog.CompletedAt = DateTime.UtcNow;
            syncLog.ErrorMessage = ex.Message;
        }

        _unitOfWork.SyncLogs.Update(syncLog);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SyncEventsAsync()
    {
        var syncLog = new SyncLog
        {
            Id = Guid.NewGuid(),
            SyncType = SyncType.Events,
            Status = SyncStatus.Running,
            StartedAt = DateTime.UtcNow
        };
        await _unitOfWork.SyncLogs.AddAsync(syncLog);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            var allUsers = await _unitOfWork.Users.GetAllAsync();
            var users = allUsers
                .Where(u => !string.IsNullOrEmpty(u.Email))
                .ToList();

            _logger.LogInformation("Syncing events for {Count} users with email", users.Count);

            var from = DateTime.UtcNow.AddDays(-7);
            var to = DateTime.UtcNow.AddDays(30);
            int processed = 0, failed = 0;
            const int batchSize = 50;

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                try
                {
                    var graphEvents = await _graphService
                        .GetUserEventsAsync(user.MicrosoftId, from, to);

                    var existingEvents = await _unitOfWork.Events.GetByUserIdAsync(user.Id);
                    var eventsInWindow = existingEvents
                        .Where(e => e.StartDateTime >= from && e.StartDateTime <= to)
                        .ToList();
                    _unitOfWork.Events.RemoveRange(eventsInWindow);

                    var newEvents = graphEvents.Select(ge => new Event
                    {
                        Id = Guid.NewGuid(),
                        MicrosoftEventId = ge.MicrosoftEventId,
                        Subject = ge.Subject,
                        BodyPreview = ge.BodyPreview?.Length > 1000
                            ? ge.BodyPreview[..1000] : ge.BodyPreview,
                        StartDateTime = ge.StartDateTime,
                        EndDateTime = ge.EndDateTime,
                        Location = ge.Location,
                        IsAllDay = ge.IsAllDay,
                        OrganizerName = ge.OrganizerName,
                        OrganizerEmail = ge.OrganizerEmail,
                        IsCancelled = ge.IsCancelled,
                        UserId = user.Id
                    }).ToList();

                    await _unitOfWork.Events.AddRangeAsync(newEvents);
                    processed += newEvents.Count;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to sync events for user {UserId}", user.Id);
                    failed++;
                }

                if ((i + 1) % batchSize == 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                    syncLog.RecordsProcessed = processed;
                    syncLog.RecordsFailed = failed;
                    _unitOfWork.SyncLogs.Update(syncLog);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Event sync progress: {Current}/{Total} users", i + 1, users.Count);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            syncLog.Status = failed > 0 ? SyncStatus.CompletedWithErrors : SyncStatus.Completed;
            syncLog.CompletedAt = DateTime.UtcNow;
            syncLog.RecordsProcessed = processed;
            syncLog.RecordsFailed = failed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Events sync failed");
            syncLog.Status = SyncStatus.Failed;
            syncLog.CompletedAt = DateTime.UtcNow;
            syncLog.ErrorMessage = ex.Message;
        }

        _unitOfWork.SyncLogs.Update(syncLog);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<SyncLogDto>> GetRecentSyncLogsAsync()
    {
        var logs = await _unitOfWork.SyncLogs.GetRecentAsync(20);
        return logs.Select(l => new SyncLogDto(
            l.Id, l.SyncType.ToString(), l.Status.ToString(),
            l.StartedAt, l.CompletedAt, l.RecordsProcessed,
            l.RecordsFailed, l.ErrorMessage));
    }

    public async Task TriggerManualSyncAsync(SyncType syncType)
    {
        switch (syncType)
        {
            case SyncType.Users:
                await SyncUsersAsync();
                break;
            case SyncType.Events:
                await SyncEventsAsync();
                break;
            case SyncType.Full:
                await SyncUsersAsync();
                await SyncEventsAsync();
                break;
        }
    }
}
