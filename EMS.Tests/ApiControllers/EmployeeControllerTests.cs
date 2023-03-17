using AutoFixture;
using AutoFixture.AutoMoq;
using EMS.Api.Controllers;
using EMS.DL.Abstractions;
using EMS.DL.Extensions;
using EMS.Dtos.ResponseDtos;
using EMS.Tests.Helpers;
using FluentAssertions;
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
        Assert.True(response.All(x => x.DepartmentId == departmentId));
        Assert.True(response.SequenceEqual(responseData));
    }
}