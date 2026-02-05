using Brainz.Application.Services;
using Brainz.Domain.Entities;
using Brainz.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Brainz.UnitTests.Application;

public class EventServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly EventService _sut;

    public EventServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new EventService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetUserEventsAsync_ReturnsPaginatedEvents()
    {
        var userId = Guid.NewGuid();
        var events = new List<Event>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Subject = "Reuniao",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                UserId = userId
            },
            new()
            {
                Id = Guid.NewGuid(),
                Subject = "Workshop",
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                UserId = userId
            }
        };

        _unitOfWorkMock.Setup(u => u.Events.GetByUserIdPagedAsync(userId, 1, 20, null, null))
            .ReturnsAsync((events.AsEnumerable(), 2));

        var result = await _sut.GetUserEventsAsync(userId, 1, 20, null, null);

        result.Should().NotBeNull();
        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetUserEventsAsync_EmptyResult_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.Events.GetByUserIdPagedAsync(userId, 1, 20, null, null))
            .ReturnsAsync((Enumerable.Empty<Event>(), 0));

        var result = await _sut.GetUserEventsAsync(userId, 1, 20, null, null);

        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserEventsAsync_MapsPropertiesCorrectly()
    {
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;
        var events = new List<Event>
        {
            new()
            {
                Id = eventId,
                Subject = "Test Event",
                BodyPreview = "Some preview",
                StartDateTime = startTime,
                EndDateTime = startTime.AddHours(1),
                Location = "Room A",
                IsAllDay = false,
                OrganizerName = "Admin",
                IsCancelled = false,
                UserId = userId
            }
        };

        _unitOfWorkMock.Setup(u => u.Events.GetByUserIdPagedAsync(userId, 1, 10, null, null))
            .ReturnsAsync((events.AsEnumerable(), 1));

        var result = await _sut.GetUserEventsAsync(userId, 1, 10, null, null);
        var dto = result.Items.First();

        dto.Id.Should().Be(eventId);
        dto.Subject.Should().Be("Test Event");
        dto.BodyPreview.Should().Be("Some preview");
        dto.Location.Should().Be("Room A");
        dto.OrganizerName.Should().Be("Admin");
        dto.IsCancelled.Should().BeFalse();
    }
}
