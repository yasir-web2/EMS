using AutoFixture;
using AutoFixture.AutoMoq;
using EMS.Api.Controllers;
using EMS.DL.Abstractions;
using EMS.DL.Extensions;
using EMS.Dtos.RequestDtos;
using EMS.Dtos.ResponseDtos;
using EMS.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EMS.Tests.ApiControllers;

public class DepartmentControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
    private readonly DepartmentController _departmentController;

    public DepartmentControllerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockDepartmentRepository = _fixture.Freeze<Mock<IDepartmentRepository>>();
        _departmentController = new DepartmentController(_mockDepartmentRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDepartments()
    {
        // Arrange
        IEnumerable<DepartmentResponse> responseData = DataHelper.GetDepartments().ToResponse();
        _mockDepartmentRepository.Setup(x => x.GetAllAsync(string.Empty, It.IsAny<CancellationToken>())).ReturnsAsync(responseData);

        // Act
        ActionResult<IEnumerable<DepartmentResponse>> actionResult = await _departmentController.GetAllAsync(string.Empty, It.IsAny<CancellationToken>());

        // Assert
        _mockDepartmentRepository.Verify();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        OkObjectResult okObjectResult = actionResult.Result.As<OkObjectResult>();
        okObjectResult.Value.Should().NotBeNull();
        IEnumerable<DepartmentResponse> response = okObjectResult.Value.As<IEnumerable<DepartmentResponse>>();
        okObjectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Count().Should().Be(2);
        Assert.True(response.SequenceEqual(responseData));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDepartmentsAgainstSearchKeyProvided()
    {
        // Arrange
        string searchKey = "HR";
        IEnumerable<DepartmentResponse> responseData = DataHelper.GetDepartments(searchKey).ToResponse();
        _mockDepartmentRepository.Setup(x => x.GetAllAsync(searchKey, It.IsAny<CancellationToken>())).ReturnsAsync(responseData);

        // Act
        ActionResult<IEnumerable<DepartmentResponse>> actionResult = await _departmentController.GetAllAsync(searchKey, It.IsAny<CancellationToken>());

        // Assert
        _mockDepartmentRepository.Verify();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        OkObjectResult okObjectResult = actionResult.Result.As<OkObjectResult>();
        okObjectResult.Value.Should().NotBeNull();
        IEnumerable<DepartmentResponse> response = okObjectResult.Value.As<IEnumerable<DepartmentResponse>>();
        okObjectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Count().Should().Be(1);
        Assert.True(response.SequenceEqual(responseData));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDepartmentAgainstIdProvided()
    {
        // Arrange
        Guid departmentId = Guid.Parse("bed64240-98e5-4b63-9469-fc961aa214c9");
        DepartmentResponse responseData = DataHelper.GetDepartment().ToResponse();
        _mockDepartmentRepository.Setup(x => x.GetByIdAsync(departmentId, It.IsAny<CancellationToken>())).ReturnsAsync(responseData);

        // Act
        ActionResult<DepartmentResponse> actionResult = await _departmentController.GetByIdAsync(departmentId, It.IsAny<CancellationToken>());

        // Assert
        _mockDepartmentRepository.Verify();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        OkObjectResult okObjectResult = actionResult.Result.As<OkObjectResult>();
        okObjectResult.Value.Should().NotBeNull();
        DepartmentResponse response = okObjectResult.Value.As<DepartmentResponse>();
        okObjectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        Assert.True(response.Id == departmentId);
        Assert.True(response.Equals(responseData));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowInvalidOperationException_OnInvalidDepartmentId()
    {
        // Arrange
        Guid departmentId = Guid.NewGuid();
        _mockDepartmentRepository.Setup(x => x.GetByIdAsync(departmentId, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException($"Department with id: {departmentId} not found."));

        // Act
        ActionResult<DepartmentResponse> actionResult = await _departmentController.GetByIdAsync(departmentId, It.IsAny<CancellationToken>());

        // Assert
        _mockDepartmentRepository.Verify();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
        BadRequestObjectResult badRequestObjectResult = actionResult.Result.As<BadRequestObjectResult>();
        badRequestObjectResult.Value.Should().NotBeNull();
        badRequestObjectResult.Value.Should().Be($"Department with id: {departmentId} not found.");
        badRequestObjectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddAsync_ShouldAddNewDepartment()
    {
        // Arrange
        DepartmentRequest request = new()
        {
            Name = "Accounts"
        };
        _mockDepartmentRepository.Setup(x => x.AddAsync(request, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        IActionResult actionResult = await _departmentController.AddAsync(request, It.IsAny<CancellationToken>());

        // Assert
        _mockDepartmentRepository.Verify();
        OkResult okResult = actionResult.As<OkResult>();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingDepartment()
    {
        // Arrange
        DepartmentRequest request = new()
        {
            Name = "Sales",
            Id = Guid.Parse("3736a738-3405-4eb3-b9ba-33ddbf1d3b76")
        };
        _mockDepartmentRepository.Setup(x => x.UpdateAsync(request, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        IActionResult actionResult = await _departmentController.UpdateAsync(request, It.IsAny<CancellationToken>());

        // Assert
        _mockDepartmentRepository.Verify();
        OkResult okResult = actionResult.As<OkResult>();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowInvalidOperationException_OnInvalidDepartmentId()
    {
        // Arrange
        DepartmentRequest request = new()
        {
            Name = "Production",
            Id = Guid.NewGuid()
        };
        _mockDepartmentRepository.Setup(x => x.UpdateAsync(request, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException($"Department with id: {request.Id} not found."));

        // Act
        IActionResult actionResult = await _departmentController.UpdateAsync(request, It.IsAny<CancellationToken>());

        // Assert
        _mockDepartmentRepository.Verify();
        BadRequestObjectResult badRequestObjectResult = actionResult.As<BadRequestObjectResult>();
        badRequestObjectResult.Value.Should().Be($"Department with id: {request.Id} not found.");
        badRequestObjectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteExistingDepartment()
    {
        // Arrange
        Guid departmentId = Guid.Parse("bed64240-98e5-4b63-9469-fc961aa214c9");
        _mockDepartmentRepository.Setup(x => x.DeleteAsync(departmentId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        IActionResult actionResult = await _departmentController.DeleteAsync(departmentId, It.IsAny<CancellationToken>());

        // Assert
        _mockDepartmentRepository.Verify();
        OkResult okResult = actionResult.As<OkResult>();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowInvalidOperationException_OnInvalidDepartmentId()
    {
        // Arrange
        Guid departmentId = Guid.NewGuid();
        _mockDepartmentRepository.Setup(x => x.DeleteAsync(departmentId, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException($"Department with id: {departmentId} not found."));

        // Act
        IActionResult actionResult = await _departmentController.DeleteAsync(departmentId, It.IsAny<CancellationToken>());

        // Assert
        _mockDepartmentRepository.Verify();
        BadRequestObjectResult badRequestObjectResult = actionResult.As<BadRequestObjectResult>();
        badRequestObjectResult.Value.Should().Be($"Department with id: {departmentId} not found.");
        badRequestObjectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}