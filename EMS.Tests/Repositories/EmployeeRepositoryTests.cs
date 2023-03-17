using EMS.DL.Abstractions;
using EMS.DL.Abstractions.Generics;
using EMS.DL.DataContext;
using EMS.DL.Implementations;
using EMS.DL.Implementations.Generics;
using EMS.Dtos.RequestDtos;
using EMS.Dtos.ResponseDtos;
using EMS.Entities;
using EMS.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace EMS.Tests.RepositoriesTests;

public class EmployeeRepositoryTests
{
    private readonly EMSDbContext _dbContext;
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeRepositoryTests()
    {
        _dbContext = new EMSDbContext(DbHelper.TestDbContextOptions());
        IRepository<Employee> repository = new Repository<Employee>(_dbContext);
        _employeeRepository = new EmployeeRepository(repository);
    }

    [Fact]
    public async Task AddAsync_ShouldAdd_NewEmployee()
    {
        // Arrange
        DbHelper.AddDepartments(_dbContext);
        EmployeeRequest request = new EmployeeRequest
        {
            Name = "Jhon",
            DateOfBirth = DateTime.Now.AddYears(-25),
            Email = "jhon@gmail.com",
            Id = Guid.NewGuid(),
            DepartmentId = Guid.Parse("bed64240-98e5-4b63-9469-fc961aa214c9")
        };

        // Act
        await _employeeRepository.AddAsync(request, It.IsAny<CancellationToken>());

        // Assert
        _dbContext.Employee.Should().NotBeNull();
        _dbContext.Employee.Count().Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdate_ExistingEmployee()
    {
        // Arrange
        DbHelper.SetupDatabase(_dbContext);
        Guid employeeIdToUpdate = Guid.Parse("187f1962-8e71-4eca-b7a3-a81a354f249b");
        Employee oldEmployee = _dbContext.Employee.Find(employeeIdToUpdate);
        string oldName = oldEmployee.Name;
        string oldEmail = oldEmployee.Email;
        EmployeeRequest request = new EmployeeRequest
        {
            Id = employeeIdToUpdate,
            Name = "Gilbert",
            Email = "gilbert@gmail.com",
            DepartmentId = Guid.Parse("3736a738-3405-4eb3-b9ba-33ddbf1d3b76"),
            DateOfBirth = DateTime.Now.AddYears(-22)
        };

        // Act
        await _employeeRepository.UpdateAsync(request, It.IsAny<CancellationToken>());

        // Assert
        Employee newEmployee = _dbContext.Employee.Find(employeeIdToUpdate);
        newEmployee.Name.Should().NotBe(oldName);
        newEmployee.Email.Should().NotBe(oldEmail);
        newEmployee.Name.Should().Be(request.Name);
        newEmployee.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_IfEmployeeIdIsInvalid()
    {
        // Arrange
        DbHelper.AddEmployees(_dbContext);
        EmployeeRequest request = new EmployeeRequest
        {
            Id = Guid.NewGuid(),
            Name = "David",
            Email = "david@gmail.com"
        };

        // Act
        async Task response() => await _employeeRepository.UpdateAsync(request, It.IsAny<CancellationToken>());

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(response);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_ExistingEmployee()
    {
        // Arrange
        DbHelper.AddEmployees(_dbContext);
        int oldEmployeesCount = _dbContext.Employee.Count();
        Guid employeeIdToDelete = Guid.Parse("3045f66f-45af-4a1b-92ad-24855e2e2826");

        // Act
        await _employeeRepository.DeleteAsync(employeeIdToDelete, It.IsAny<CancellationToken>());

        // Assert
        Employee employee = _dbContext.Employee.Find(employeeIdToDelete);
        employee.Should().BeNull();
        oldEmployeesCount.Should().Be(2);
        _dbContext.Employee.Count().Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_IfEmployeeIdIsInvalid()
    {
        // Arrange
        DbHelper.AddEmployees(_dbContext);

        // Act
        async Task response() => await _employeeRepository.DeleteAsync(Guid.NewGuid(), It.IsAny<CancellationToken>());

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(response);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEmployeeAgainstGivenId()
    {
        // Arrange
        DbHelper.SetupDatabase(_dbContext);
        Guid idToFetch = Guid.Parse("3045f66f-45af-4a1b-92ad-24855e2e2826");

        // Act
        EmployeeResponse employee = await _employeeRepository.GetByIdAsync(idToFetch, It.IsAny<CancellationToken>());

        // Assert
        employee.Should().NotBeNull();
        employee.Id.Should().Be(idToFetch);
        employee.Name.Should().Be("Jhon");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowException_IfEmployeeIdIsInvalid()
    {
        // Arrange
        DbHelper.AddEmployees(_dbContext);

        // Act
        async Task response() => await _employeeRepository.GetByIdAsync(Guid.NewGuid(), It.IsAny<CancellationToken>());

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(response);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEmployees()
    {
        // Arrange
        DbHelper.SetupDatabase(_dbContext);

        // Act
        IEnumerable<EmployeeResponse> employees = await _employeeRepository.GetAllAsync(string.Empty, It.IsAny<CancellationToken>());

        // Assert
        employees.Should().NotBeNull();
        employees.Any().Should().Be(true);
        employees.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEmployeesAgainstSearchKeyProvided()
    {
        // Arrange
        DbHelper.SetupDatabase(_dbContext);
        string searchKey = "william";

        // Act
        IEnumerable<EmployeeResponse> employees = await _employeeRepository.GetAllAsync(searchKey, It.IsAny<CancellationToken>());

        // Assert
        employees.Should().NotBeNull();
        employees.Any().Should().Be(true);
        employees.Count().Should().Be(1);
        employees.FirstOrDefault().Email.Should().Contain(searchKey);
    }
}