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

public class EmployeeControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IEmployeeRepository> _mockEmployeeRepository;
    private readonly EmployeeController _employeeController;

    public EmployeeControllerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockEmployeeRepository = _fixture.Freeze<Mock<IEmployeeRepository>>();
        _employeeController = new EmployeeController(_mockEmployeeRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEmployees()
    {
        // Arrange
        IEnumerable<EmployeeResponse> responseData = DataHelper.GetEmployees().ToResponse();
        _mockEmployeeRepository.Setup(x => x.GetAllAsync(string.Empty, It.IsAny<CancellationToken>())).ReturnsAsync(responseData);

        // Act
        ActionResult<IEnumerable<EmployeeResponse>> actionResult = await _employeeController.GetAllAsync(string.Empty, It.IsAny<CancellationToken>());

        // Assert
        _mockEmployeeRepository.Verify();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        OkObjectResult okObjectResult = actionResult.Result.As<OkObjectResult>();
        okObjectResult.Value.Should().NotBeNull();
        IEnumerable<EmployeeResponse> response = okObjectResult.Value.As<IEnumerable<EmployeeResponse>>();
        okObjectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Count().Should().Be(2);
        Assert.True(response.SequenceEqual(responseData));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEmployeesAgainstSearchKeyProvided()
    {
        // Arrange
        string searchKey = "will";
        IEnumerable<EmployeeResponse> responseData = DataHelper.GetEmployees(searchKey).ToResponse();
        _mockEmployeeRepository.Setup(x => x.GetAllAsync(searchKey, It.IsAny<CancellationToken>())).ReturnsAsync(responseData);

        // Act
        ActionResult<IEnumerable<EmployeeResponse>> actionResult = await _employeeController.GetAllAsync(searchKey, It.IsAny<CancellationToken>());

        // Assert
        _mockEmployeeRepository.Verify();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        OkObjectResult okObjectResult = actionResult.Result.As<OkObjectResult>();
        okObjectResult.Value.Should().NotBeNull();
        IEnumerable<EmployeeResponse> response = okObjectResult.Value.As<IEnumerable<EmployeeResponse>>();
        okObjectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Count().Should().Be(1);
        Assert.True(response.SequenceEqual(responseData));
    }

    [Fact]
    public async Task GetAllByDepartmentIdAsync_ShouldReturnAllEmployeesAgainstDepartmentIdProvided()
    {
        // Arrange
        Guid departmentId = Guid.Parse("3736a738-3405-4eb3-b9ba-33ddbf1d3b76");
        IEnumerable<EmployeeResponse> responseData = DataHelper.GetEmployees().ToResponse().Where(x => x.DepartmentId == departmentId);
        _mockEmployeeRepository.Setup(x => x.GetAllByDepartmentAsync(departmentId, It.IsAny<CancellationToken>())).ReturnsAsync(responseData);

        // Act
        ActionResult<IEnumerable<EmployeeResponse>> actionResult = await _employeeController.GetAllByDepartmentIdAsync(departmentId, It.IsAny<CancellationToken>());

        // Assert
        _mockEmployeeRepository.Verify();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        OkObjectResult okObjectResult = actionResult.Result.As<OkObjectResult>();
        okObjectResult.Value.Should().NotBeNull();
        IEnumerable<EmployeeResponse> response = okObjectResult.Value.As<IEnumerable<EmployeeResponse>>();
        okObjectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        Assert.True(response.All(x => x.DepartmentId == departmentId));
        Assert.True(response.SequenceEqual(responseData));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEmployeeAgainstIdProvided()
    {
        // Arrange
        Guid employeeId = Guid.Parse("3045f66f-45af-4a1b-92ad-24855e2e2826");
        EmployeeResponse responseData = DataHelper.GetEmployee().ToResponse();
        _mockEmployeeRepository.Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>())).ReturnsAsync(responseData);

        // Act
        ActionResult<EmployeeResponse> actionResult = await _employeeController.GetByIdAsync(employeeId, It.IsAny<CancellationToken>());

        // Assert
        _mockEmployeeRepository.Verify();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        OkObjectResult okObjectResult = actionResult.Result.As<OkObjectResult>();
        okObjectResult.Value.Should().NotBeNull();
        EmployeeResponse response = okObjectResult.Value.As<EmployeeResponse>();
        okObjectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        Assert.True(response.Id == employeeId);
        Assert.True(response.Equals(responseData));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowInvalidOperationException_OnInvalidEmployeeId()
    {
        // Arrange
        Guid employeeId = Guid.NewGuid();
        _mockEmployeeRepository.Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException($"Employee with id: {employeeId} not found."));

        // Act
        ActionResult<EmployeeResponse> actionResult = await _employeeController.GetByIdAsync(employeeId, It.IsAny<CancellationToken>());

        // Assert
        _mockEmployeeRepository.Verify();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
        BadRequestObjectResult badRequestObjectResult = actionResult.Result.As<BadRequestObjectResult>();
        badRequestObjectResult.Value.Should().NotBeNull();
        badRequestObjectResult.Value.Should().Be($"Employee with id: {employeeId} not found.");
        badRequestObjectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddAsync_ShouldAddNewEmployee()
    {
        // Arrange
        EmployeeRequest request = new()
        {
            DateOfBirth = DateTime.Now.AddYears(-21),
            DepartmentId = Guid.Parse("3736a738-3405-4eb3-b9ba-33ddbf1d3b76"),
            Email = "gilbert@gmail.com",
            Name = "Gilbert"
        };
        _mockEmployeeRepository.Setup(x => x.AddAsync(request, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        IActionResult actionResult = await _employeeController.AddAsync(request, It.IsAny<CancellationToken>());

        // Assert
        _mockEmployeeRepository.Verify();
        OkResult okResult = actionResult.As<OkResult>();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingEmployee()
    {
        // Arrange
        EmployeeRequest request = new()
        {
            DateOfBirth = DateTime.Now.AddYears(-22),
            DepartmentId = Guid.Parse("bed64240-98e5-4b63-9469-fc961aa214c9"),
            Email = "william@gmail.com",
            Name = "William",
            Id = Guid.Parse("187f1962-8e71-4eca-b7a3-a81a354f249b")
        };
        _mockEmployeeRepository.Setup(x => x.UpdateAsync(request, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        IActionResult actionResult = await _employeeController.UpdateAsync(request, It.IsAny<CancellationToken>());

        // Assert
        _mockEmployeeRepository.Verify();
        OkResult okResult = actionResult.As<OkResult>();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowInvalidOperationException_OnInvalidEmployeeId()
    {
        // Arrange
        EmployeeRequest request = new()
        {
            DateOfBirth = DateTime.Now.AddYears(-22),
            DepartmentId = Guid.Parse("bed64240-98e5-4b63-9469-fc961aa214c9"),
            Email = "william@gmail.com",
            Name = "William",
            Id = Guid.NewGuid()
        };
        _mockEmployeeRepository.Setup(x => x.UpdateAsync(request, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException($"Employee with id: {request.Id} not found."));

        // Act
        IActionResult actionResult = await _employeeController.UpdateAsync(request, It.IsAny<CancellationToken>());

        // Assert
        _mockEmployeeRepository.Verify();
        BadRequestObjectResult badRequestObjectResult = actionResult.As<BadRequestObjectResult>();
        badRequestObjectResult.Value.Should().Be($"Employee with id: {request.Id} not found.");
        badRequestObjectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteExistingEmployee()
    {
        // Arrange
        Guid employeeId = Guid.Parse("187f1962-8e71-4eca-b7a3-a81a354f249b");
        _mockEmployeeRepository.Setup(x => x.DeleteAsync(employeeId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        IActionResult actionResult = await _employeeController.DeleteAsync(employeeId, It.IsAny<CancellationToken>());

        // Assert
        _mockEmployeeRepository.Verify();
        OkResult okResult = actionResult.As<OkResult>();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowInvalidOperationException_OnInvalidEmployeeId()
    {
        // Arrange
        Guid employeeId = Guid.NewGuid();
        _mockEmployeeRepository.Setup(x => x.DeleteAsync(employeeId, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException($"Employee with id: {employeeId} not found."));

        // Act
        IActionResult actionResult = await _employeeController.DeleteAsync(employeeId, It.IsAny<CancellationToken>());

        // Assert
        _mockEmployeeRepository.Verify();
        BadRequestObjectResult badRequestObjectResult = actionResult.As<BadRequestObjectResult>();
        badRequestObjectResult.Value.Should().Be($"Employee with id: {employeeId} not found.");
        badRequestObjectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}