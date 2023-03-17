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

public class DepartmentRepositoryTests
{
    private readonly EMSDbContext _dbContext;
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentRepositoryTests()
    {
        _dbContext = new EMSDbContext(DbHelper.TestDbContextOptions());
        IRepository<Department> repository = new Repository<Department>(_dbContext);
        _departmentRepository = new DepartmentRepository(repository);
    }

    [Fact]
    public async Task AddAsync_ShouldAdd_NewDepartment()
    {
        // Arrange
        DepartmentRequest request = new DepartmentRequest
        {
            Name = "HR"
        };

        // Act
        await _departmentRepository.AddAsync(request, It.IsAny<CancellationToken>());

        // Assert
        _dbContext.Department.Should().NotBeNull();
        _dbContext.Department.Count().Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdate_ExistingDepartment()
    {
        // Arrange
        DbHelper.AddDepartments(_dbContext);
        Guid departmentIdToUpdate = Guid.Parse("bed64240-98e5-4b63-9469-fc961aa214c9");
        string departmentOldName = _dbContext.Department.Find(departmentIdToUpdate).Name;
        DepartmentRequest request = new DepartmentRequest
        {
            Id = departmentIdToUpdate,
            Name = "Accounts"
        };

        // Act
        await _departmentRepository.UpdateAsync(request, It.IsAny<CancellationToken>());

        // Assert
        string departmentNewName = _dbContext.Department.Find(departmentIdToUpdate).Name;
        departmentNewName.Should().NotBe(departmentOldName);
        departmentNewName.Should().Be(request.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_IfDepartmentIdIsInvalid()
    {
        // Arrange
        DbHelper.AddDepartments(_dbContext);
        DepartmentRequest request = new DepartmentRequest
        {
            Id = Guid.NewGuid(),
            Name = "Accounts"
        };

        // Act
        async Task response() => await _departmentRepository.UpdateAsync(request, It.IsAny<CancellationToken>());

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(response);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_ExistingDepartment()
    {
        // Arrange
        DbHelper.AddDepartments(_dbContext);
        int oldDepartmentsCount = _dbContext.Department.Count();
        Guid departmentIdToDelete = Guid.Parse("bed64240-98e5-4b63-9469-fc961aa214c9");

        // Act
        await _departmentRepository.DeleteAsync(departmentIdToDelete, It.IsAny<CancellationToken>());

        // Assert
        Department department = _dbContext.Department.Find(departmentIdToDelete);
        department.Should().BeNull();
        oldDepartmentsCount.Should().Be(2);
        _dbContext.Department.Count().Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_IfDepartmentIdIsInvalid()
    {
        // Arrange
        DbHelper.AddDepartments(_dbContext);

        // Act
        async Task response() => await _departmentRepository.DeleteAsync(Guid.NewGuid(), It.IsAny<CancellationToken>());

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(response);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDepartmentAgainstGivenId()
    {
        // Arrange
        DbHelper.AddDepartments(_dbContext);
        Guid idToFetch = Guid.Parse("3736a738-3405-4eb3-b9ba-33ddbf1d3b76");

        // Act
        DepartmentResponse department = await _departmentRepository.GetByIdAsync(idToFetch, It.IsAny<CancellationToken>());

        // Assert
        department.Should().NotBeNull();
        department.Id.Should().Be(idToFetch);
        department.Name.Should().Be("IT");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowException_IfDepartmentIdIsInvalid()
    {
        // Arrange
        DbHelper.AddDepartments(_dbContext);

        // Act
        async Task response() => await _departmentRepository.GetByIdAsync(Guid.NewGuid(), It.IsAny<CancellationToken>());

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(response);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDepartments()
    {
        // Arrange
        DbHelper.AddDepartments(_dbContext);

        // Act
        IEnumerable<DepartmentResponse> departments = await _departmentRepository.GetAllAsync(string.Empty, It.IsAny<CancellationToken>());

        // Assert
        departments.Should().NotBeNull();
        departments.Any().Should().Be(true);
        departments.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDepartmentsAgainstSearchKeyProvided()
    {
        // Arrange
        DbHelper.AddDepartments(_dbContext);
        string searchKey = "HR";

        // Act
        IEnumerable<DepartmentResponse> departments = await _departmentRepository.GetAllAsync(searchKey, It.IsAny<CancellationToken>());

        // Assert
        departments.Should().NotBeNull();
        departments.Any().Should().Be(true);
        departments.Count().Should().Be(1);
        departments.FirstOrDefault().Name.Should().Contain(searchKey);
    }
}