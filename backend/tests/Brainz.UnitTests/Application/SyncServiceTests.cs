using Brainz.Application.DTOs;
using Brainz.Application.Interfaces;
using Brainz.Application.Services;
using Brainz.Domain.Entities;
using Brainz.Domain.Enums;
using Brainz.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Brainz.UnitTests.Application;

public class SyncServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMicrosoftGraphService> _graphServiceMock;
    private readonly Mock<ILogger<SyncService>> _loggerMock;
    private readonly SyncService _sut;

    public SyncServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _graphServiceMock = new Mock<IMicrosoftGraphService>();
        _loggerMock = new Mock<ILogger<SyncService>>();

        _unitOfWorkMock.Setup(u => u.SyncLogs.AddAsync(It.IsAny<SyncLog>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SyncLogs.IsRunningAsync(It.IsAny<SyncType>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        _sut = new SyncService(
            _unitOfWorkMock.Object,
            _graphServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SyncUsersAsync_NewUsers_AddsToRepository()
    {
        var graphUsers = new List<GraphUserDto>
        {
            new() { MicrosoftId = "ms-1", DisplayName = "John", Email = "john@edu.org" }
        };

        _graphServiceMock.Setup(g => g.GetAllUsersAsync()).ReturnsAsync(graphUsers);
        _unitOfWorkMock.Setup(u => u.Users.GetByMicrosoftIdAsync("ms-1"))
            .ReturnsAsync((User?)null);
        _unitOfWorkMock.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        await _sut.SyncUsersAsync();

        _unitOfWorkMock.Verify(
            u => u.Users.AddAsync(It.Is<User>(usr => usr.MicrosoftId == "ms-1")),
            Times.Once);
    }

    [Fact]
    public async Task SyncUsersAsync_ExistingUser_UpdatesInRepository()
    {
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            MicrosoftId = "ms-1",
            DisplayName = "Old Name",
            Email = "old@edu.org"
        };

        var graphUsers = new List<GraphUserDto>
        {
            new() { MicrosoftId = "ms-1", DisplayName = "New Name", Email = "new@edu.org" }
        };

        _graphServiceMock.Setup(g => g.GetAllUsersAsync()).ReturnsAsync(graphUsers);
        _unitOfWorkMock.Setup(u => u.Users.GetByMicrosoftIdAsync("ms-1"))
            .ReturnsAsync(existingUser);

        await _sut.SyncUsersAsync();

        existingUser.DisplayName.Should().Be("New Name");
        existingUser.Email.Should().Be("new@edu.org");
        _unitOfWorkMock.Verify(u => u.Users.Update(existingUser), Times.Once);
    }

    [Fact]
    public async Task SyncUsersAsync_GraphThrows_LogsFailed()
    {
        _graphServiceMock.Setup(g => g.GetAllUsersAsync())
            .ThrowsAsync(new HttpRequestException("Graph API unavailable"));

        await _sut.SyncUsersAsync();

        _unitOfWorkMock.Verify(
            u => u.SyncLogs.Update(It.Is<SyncLog>(s => s.Status == SyncStatus.Failed)),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task SyncUsersAsync_CompletesSuccessfully_LogsCompleted()
    {
        _graphServiceMock.Setup(g => g.GetAllUsersAsync())
            .ReturnsAsync(new List<GraphUserDto>());

        await _sut.SyncUsersAsync();

        _unitOfWorkMock.Verify(
            u => u.SyncLogs.Update(It.Is<SyncLog>(s => s.Status == SyncStatus.Completed)),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task SyncEventsAsync_FetchesEventsForEachUser()
    {
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), MicrosoftId = "ms-1", Email = "user1@edu.org" },
            new() { Id = Guid.NewGuid(), MicrosoftId = "ms-2", Email = "user2@edu.org" }
        };

        _unitOfWorkMock.Setup(u => u.Users.GetAllAsync()).ReturnsAsync(users);
        _unitOfWorkMock.Setup(u => u.Events.GetByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Event>());
        _unitOfWorkMock.Setup(u => u.Events.AddRangeAsync(It.IsAny<IEnumerable<Event>>()))
            .Returns(Task.CompletedTask);

        _graphServiceMock.Setup(g => g.GetUserEventsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<GraphEventDto>());

        await _sut.SyncEventsAsync();

        _graphServiceMock.Verify(
            g => g.GetUserEventsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task GetRecentSyncLogsAsync_ReturnsMappedDtos()
    {
        var logs = new List<SyncLog>
        {
            new()
            {
                Id = Guid.NewGuid(),
                SyncType = SyncType.Users,
                Status = SyncStatus.Completed,
                StartedAt = DateTime.UtcNow.AddHours(-1),
                CompletedAt = DateTime.UtcNow,
                RecordsProcessed = 10,
                RecordsFailed = 0
            }
        };

        _unitOfWorkMock.Setup(u => u.SyncLogs.GetRecentAsync(20)).ReturnsAsync(logs);

        var result = await _sut.GetRecentSyncLogsAsync();

        result.Should().HaveCount(1);
        var dto = result.First();
        dto.SyncType.Should().Be("Users");
        dto.Status.Should().Be("Completed");
        dto.RecordsProcessed.Should().Be(10);
    }
}
