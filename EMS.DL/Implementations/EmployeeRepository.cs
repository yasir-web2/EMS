using EMS.DL.Abstractions;
using EMS.DL.Abstractions.Generics;
using EMS.DL.Extensions;
using EMS.Dtos.RequestDtos;
using EMS.Dtos.ResponseDtos;
using EMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMS.DL.Implementations;

public sealed class EmployeeRepository : IEmployeeRepository
{
    #region Private Fields

    private readonly IRepository<Employee> _employeeRepository;

    #endregion

    #region Constructor

    public EmployeeRepository(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    #endregion

    #region Public Methods

    public async Task AddAsync(EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        Employee employee = (Employee)request;
        await _employeeRepository.AddWithSaveAsync(employee, cancellationToken);
    }

    public async Task UpdateAsync(EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        Employee? employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new InvalidOperationException($"Employee with id: {request.Id} not found.");

        employee.UpdatedDate = DateTime.Now;
        employee.Name = request.Name;
        employee.DateOfBirth = request.DateOfBirth;
        employee.DepartmentId = request.DepartmentId;
        employee.Email = request.Email;
        await _employeeRepository.UpdateWithSaveAsync(employee, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Employee? employee = await _employeeRepository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Employee with id: {id} not found.");

        await _employeeRepository.DeleteWithSaveAsync(employee, cancellationToken);
    }

    public async Task<EmployeeResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Employee? employee = await _employeeRepository.GetByConditionAsync(x => x.Id == id, cancellationToken, a => a.Department) ?? throw new InvalidOperationException($"Employee with id: {id} not found.");

        return employee.ToResponse();
    }

    public async Task<IEnumerable<EmployeeResponse>> GetAllAsync(string? searchKey = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Employee> employeesQuery = _employeeRepository.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchKey))
            employeesQuery = employeesQuery.Where(x => x.Name.Contains(searchKey) || x.Email.Contains(searchKey));

        List<Employee> employees = await employeesQuery.Include(a => a.Department).AsNoTracking().ToListAsync(cancellationToken);
        return employees.ToResponse();
    }

    public async Task<IEnumerable<EmployeeResponse>> GetAllByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        IQueryable<Employee> employeesQuery = _employeeRepository.AsQueryable(x => x.DepartmentId == departmentId);
        List<Employee> employees = await employeesQuery.AsNoTracking().ToListAsync(cancellationToken);
        return employees.ToResponse();
    }

    #endregion
}