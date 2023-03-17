using EMS.DL.Abstractions;
using EMS.DL.Abstractions.Generics;
using EMS.DL.Extensions;
using EMS.Dtos.RequestDtos;
using EMS.Dtos.ResponseDtos;
using EMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMS.DL.Implementations;

public sealed class DepartmentRepository : IDepartmentRepository
{
    #region Private Fields

    private readonly IRepository<Department> _departmentRepository;

    #endregion

    #region Constructor

    public DepartmentRepository(IRepository<Department> departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    #endregion

    #region Public Methods

    public async Task AddAsync(DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        Department department = (Department)request;
        await _departmentRepository.AddWithSaveAsync(department, cancellationToken);
    }

    public async Task UpdateAsync(DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        Department? department = await _departmentRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new InvalidOperationException($"Department with id: {request.Id} not found.");

        department.UpdatedDate = DateTime.Now;
        department.Name = request.Name;
        await _departmentRepository.UpdateWithSaveAsync(department, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Department? department = await _departmentRepository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Department with id: {id} not found.");

        await _departmentRepository.DeleteWithSaveAsync(department, cancellationToken);
    }

    public async Task<DepartmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Department? department = await _departmentRepository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Department with id: {id} not found.");

        return department.ToResponse();
    }

    public async Task<IEnumerable<DepartmentResponse>> GetAllAsync(string? searchKey = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Department> departmentsQuery = _departmentRepository.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchKey))
            departmentsQuery = departmentsQuery.Where(x => x.Name.Contains(searchKey));

        List<Department> departments = await departmentsQuery.AsNoTracking().ToListAsync(cancellationToken);
        return departments.ToResponse();
    }

    public async Task<IEnumerable<DropdownResponse>> GetDropdownAsync(CancellationToken cancellationToken = default)
    {
        return await _departmentRepository.AsQueryable().Select(x => new DropdownResponse
        {
            Id = x.Id,
            Name = x.Name
        }).AsNoTracking().ToListAsync(cancellationToken);
    }

    #endregion
}