using Brainz.Application.Services;
using Brainz.Domain.Entities;
using Brainz.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Brainz.UnitTests.Application;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new UserService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsPaginatedResults()
    {
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), DisplayName = "John", Email = "john@test.com", IsActive = true },
            new() { Id = Guid.NewGuid(), DisplayName = "Jane", Email = "jane@test.com", IsActive = true }
        };

        _unitOfWorkMock.Setup(u => u.Users.GetPagedAsync(1, 20, null))
            .ReturnsAsync((users.AsEnumerable(), 2));

        var result = await _sut.GetUsersAsync(1, 20, null);

        result.Should().NotBeNull();
        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task GetUsersAsync_WithSearch_PassesSearchTerm()
    {
        _unitOfWorkMock.Setup(u => u.Users.GetPagedAsync(1, 20, "john"))
            .ReturnsAsync((Enumerable.Empty<User>(), 0));

        var result = await _sut.GetUsersAsync(1, 20, "john");

        result.TotalCount.Should().Be(0);
        _unitOfWorkMock.Verify(u => u.Users.GetPagedAsync(1, 20, "john"), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_ReturnsDto()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            DisplayName = "John Doe",
            Email = "john@test.com",
            Department = "IT",
            JobTitle = "Dev",
            IsActive = true
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var result = await _sut.GetUserByIdAsync(userId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.DisplayName.Should().Be("John Doe");
        result.Email.Should().Be("john@test.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_NonExistingUser_ReturnsNull()
    {
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        var result = await _sut.GetUserByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
}
